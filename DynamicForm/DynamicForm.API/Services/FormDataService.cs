using DynamicForm.API.Data;
using DynamicForm.API.DTOs;
using DynamicForm.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DynamicForm.API.Services;

public class FormDataService : IFormDataService
{
    private readonly ApplicationDbContext _context;
    private readonly IFormService _formService;

    public FormDataService(ApplicationDbContext context, IFormService formService)
    {
        _context = context;
        _formService = formService;
    }

    public async Task<FormDataDto?> GetFormDataAsync(int submissionId)
    {
        // Query FormDataValues trực tiếp bằng SubmissionId (INT)
        var values = await _context.FormDataValues
            .Include(v => v.FormVersion)
            .Include(v => v.FormField)
            .Where(v => v.SubmissionId == submissionId)
            .OrderBy(v => v.DisplayOrder)
            .ToListAsync();

        if (values.Count == 0) return null;

        var firstValue = values.First();
        
        // Group FormDataValues by FieldCode
        var dataDict = new Dictionary<string, object>();
        
        foreach (var value in values)
        {
            var fieldCode = value.FormField.FieldCode;
            
            // Nếu field có MaxOccurs > 1 (repeat section), lưu thành array
            if (value.FormField.MaxOccurs > 1)
            {
                if (!dataDict.ContainsKey(fieldCode))
                {
                    dataDict[fieldCode] = new List<object>();
                }
                ((List<object>)dataDict[fieldCode]).Add(value.FieldValue ?? string.Empty);
            }
            else
            {
                // Field thường, chỉ lưu giá trị cuối cùng (nếu có nhiều)
                dataDict[fieldCode] = value.FieldValue ?? string.Empty;
            }
        }

        return new FormDataDto
        {
            Id = firstValue.PublicId, // Return FormDataValue PublicId (hoặc có thể dùng SubmissionId)
            FormVersionId = firstValue.FormVersion.PublicId, // Return FormVersion PublicId
            ObjectId = firstValue.ObjectId,
            ObjectType = firstValue.ObjectType,
            Data = dataDict,
            CreatedDate = firstValue.CreatedDate,
            CreatedBy = firstValue.CreatedBy,
            ModifiedDate = firstValue.ModifiedDate,
            ModifiedBy = firstValue.ModifiedBy,
            Status = firstValue.Status
        };
    }

    public async Task<FormDataDto?> GetFormDataByObjectAsync(string objectId, string objectType, Guid formVersionPublicId)
    {
        // Convert FormVersion PublicId -> FormVersion Id
        var formVersionId = await _context.FormVersions
            .Where(v => v.PublicId == formVersionPublicId)
            .Select(v => (int?)v.Id)
            .FirstOrDefaultAsync();
        
        if (formVersionId == null) return null;

        // Lấy SubmissionId mới nhất của object này
        var latestSubmissionId = await _context.FormDataValues
            .Where(v => v.ObjectId == objectId && v.ObjectType == objectType && v.FormVersionId == formVersionId.Value)
            .OrderByDescending(v => v.CreatedDate)
            .Select(v => (int?)v.SubmissionId)
            .FirstOrDefaultAsync();

        if (latestSubmissionId == null) return null;

        return await GetFormDataAsync(latestSubmissionId.Value);
    }

    public async Task<FormDataDto> CreateFormDataAsync(CreateFormDataRequest request)
    {
        // Validate data based on FormFields rules
        var validationResult = await ValidateFormDataAsync(request.FormVersionId, request.Data);
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.Message))}");
        }

        // Convert FormVersion PublicId -> FormVersion Id
        var version = await _context.FormVersions
            .Include(v => v.Fields)
            .FirstOrDefaultAsync(v => v.PublicId == request.FormVersionId);
        if (version == null)
        {
            throw new InvalidOperationException("FormVersion not found");
        }
        if (version.Status != 1) // Not Published
        {
            throw new InvalidOperationException($"Cannot insert data into FormVersion with Status={version.Status}. Version must be Published (Status=1).");
        }

        // Tạo SubmissionId mới (INT) - Tự quản lý, không có bảng Submissions
        // Lấy SubmissionId lớn nhất hiện tại, +1 để tạo mới
        var maxSubmissionId = await _context.FormDataValues
            .MaxAsync(v => (int?)v.SubmissionId);
        
        var newSubmissionId = (maxSubmissionId ?? 0) + 1;
        var createdDate = DateTime.UtcNow;
        var createdBy = "System"; // TODO: Get from current user

        // Create FormDataValue cho từng field value
        var fieldsByCode = version.Fields.ToDictionary(f => f.FieldCode, f => f);
        
        foreach (var kvp in request.Data)
        {
            var fieldCode = kvp.Key;
            var fieldValue = kvp.Value;
            
            if (!fieldsByCode.TryGetValue(fieldCode, out var field))
            {
                continue; // Skip field không tồn tại trong metadata
            }

            // Xử lý repeat section (MaxOccurs > 1)
            if (field.MaxOccurs > 1 && fieldValue is IEnumerable<object> arrayValue)
            {
                // Nếu là array, tạo nhiều FormDataValue
                int displayOrder = 0;
                foreach (var item in arrayValue)
                {
                    var dataValue = new FormDataValue
                    {
                        SubmissionId = newSubmissionId, // Dùng SubmissionId mới
                        FormVersionId = version.Id, // Dùng version.Id (int)
                        FormFieldId = field.Id, // Dùng field.Id (int)
                        ObjectId = request.ObjectId,
                        ObjectType = request.ObjectType,
                        FieldValue = item?.ToString(),
                        DisplayOrder = displayOrder++,
                        SectionCode = field.SectionCode,
                        Status = 0, // Draft
                        CreatedBy = createdBy,
                        CreatedDate = createdDate
                    };
                    _context.FormDataValues.Add(dataValue);
                }
            }
            else
            {
                // Field thường, chỉ 1 giá trị
                var dataValue = new FormDataValue
                {
                    SubmissionId = newSubmissionId, // Dùng SubmissionId mới
                    FormVersionId = version.Id, // Dùng version.Id (int)
                    FormFieldId = field.Id, // Dùng field.Id (int)
                    ObjectId = request.ObjectId,
                    ObjectType = request.ObjectType,
                    FieldValue = fieldValue?.ToString(),
                    DisplayOrder = 0,
                    SectionCode = field.SectionCode,
                    Status = 0, // Draft
                    CreatedBy = createdBy,
                    CreatedDate = createdDate
                };
                _context.FormDataValues.Add(dataValue);
            }
        }

        await _context.SaveChangesAsync();

        return await GetFormDataAsync(newSubmissionId) ?? throw new Exception("Failed to retrieve created form data");
    }

    public async Task<FormDataDto> UpdateFormDataAsync(int submissionId, CreateFormDataRequest request)
    {
        // Query FormDataValues trực tiếp bằng SubmissionId (INT)
        var oldValues = await _context.FormDataValues
            .Where(v => v.SubmissionId == submissionId)
            .ToListAsync();

        if (oldValues.Count == 0) throw new ArgumentException("Form data not found");

        // Validate data
        var validationResult = await ValidateFormDataAsync(request.FormVersionId, request.Data);
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.Message))}");
        }

        // Convert FormVersion PublicId -> FormVersion Id
        var version = await _context.FormVersions
            .Include(v => v.Fields)
            .FirstOrDefaultAsync(v => v.PublicId == request.FormVersionId);
        if (version == null)
        {
            throw new InvalidOperationException("FormVersion not found");
        }

        var firstOldValue = oldValues.First();
        var modifiedDate = DateTime.UtcNow;
        var modifiedBy = "System"; // TODO: Get from current user

        // Delete existing FormDataValues
        _context.FormDataValues.RemoveRange(oldValues);

        // Create new FormDataValues với cùng SubmissionId
        var fieldsByCode = version.Fields.ToDictionary(f => f.FieldCode, f => f);
        
        foreach (var kvp in request.Data)
        {
            var fieldCode = kvp.Key;
            var fieldValue = kvp.Value;
            
            if (!fieldsByCode.TryGetValue(fieldCode, out var field))
            {
                continue; // Skip field không tồn tại trong metadata
            }

            // Xử lý repeat section (MaxOccurs > 1)
            if (field.MaxOccurs > 1 && fieldValue is IEnumerable<object> arrayValue)
            {
                int displayOrder = 0;
                foreach (var item in arrayValue)
                {
                    var dataValue = new FormDataValue
                    {
                        SubmissionId = submissionId, // Dùng SubmissionId cũ
                        FormVersionId = version.Id, // Dùng version.Id (int)
                        FormFieldId = field.Id, // Dùng field.Id (int)
                        ObjectId = request.ObjectId,
                        ObjectType = request.ObjectType,
                        FieldValue = item?.ToString(),
                        DisplayOrder = displayOrder++,
                        SectionCode = field.SectionCode,
                        Status = firstOldValue.Status, // Giữ nguyên status
                        CreatedBy = firstOldValue.CreatedBy, // Giữ nguyên createdBy
                        CreatedDate = firstOldValue.CreatedDate, // Giữ nguyên createdDate
                        ModifiedBy = modifiedBy,
                        ModifiedDate = modifiedDate
                    };
                    _context.FormDataValues.Add(dataValue);
                }
            }
            else
            {
                var dataValue = new FormDataValue
                {
                    SubmissionId = submissionId, // Dùng SubmissionId cũ
                    FormVersionId = version.Id, // Dùng version.Id (int)
                    FormFieldId = field.Id, // Dùng field.Id (int)
                    ObjectId = request.ObjectId,
                    ObjectType = request.ObjectType,
                    FieldValue = fieldValue?.ToString(),
                    DisplayOrder = 0,
                    SectionCode = field.SectionCode,
                    Status = firstOldValue.Status, // Giữ nguyên status
                    CreatedBy = firstOldValue.CreatedBy, // Giữ nguyên createdBy
                    CreatedDate = firstOldValue.CreatedDate, // Giữ nguyên createdDate
                    ModifiedBy = modifiedBy,
                    ModifiedDate = modifiedDate
                };
                _context.FormDataValues.Add(dataValue);
            }
        }

        await _context.SaveChangesAsync();

        return await GetFormDataAsync(submissionId) ?? throw new Exception("Failed to retrieve updated form data");
    }

    public async Task<ValidationResultDto> ValidateFormDataAsync(Guid formVersionId, Dictionary<string, object> data)
    {
        var metadata = await _formService.GetFormMetadataByVersionIdAsync(formVersionId);
        if (metadata == null)
        {
            return new ValidationResultDto
            {
                IsValid = false,
                Errors = new List<ValidationErrorDto> { new() { FieldCode = "", Message = "Form version not found" } }
            };
        }

        var errors = new List<ValidationErrorDto>();

        foreach (var field in metadata.Fields.Where(f => f.IsVisible))
        {
            var fieldValue = data.ContainsKey(field.FieldCode) ? data[field.FieldCode] : null;

            // Required validation
            if (field.IsRequired && (fieldValue == null || string.IsNullOrWhiteSpace(fieldValue.ToString())))
            {
                errors.Add(new ValidationErrorDto
                {
                    FieldCode = field.FieldCode,
                    Message = $"{field.Label} là bắt buộc"
                });
                continue;
            }

            // Field validations
            foreach (var validation in field.Validations)
            {
                var error = ValidateField(field, fieldValue, validation);
                if (error != null)
                {
                    errors.Add(error);
                }
            }
        }

        return new ValidationResultDto
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };
    }

    private ValidationErrorDto? ValidateField(FormFieldDto field, object? value, FieldValidationDto validation)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString())) return null;

        return validation.RuleType switch
        {
            1 => null, // Required - already checked
            2 => ValidateMin(field, value, validation), // Min
            3 => ValidateMax(field, value, validation), // Max
            4 => ValidateRange(field, value, validation), // Range
            5 => ValidateRegex(field, value, validation), // Regex
            _ => null
        };
    }

    private ValidationErrorDto? ValidateMin(FormFieldDto field, object value, FieldValidationDto validation)
    {
        if (double.TryParse(value.ToString(), out var numValue) && 
            double.TryParse(validation.RuleValue, out var minValue) && 
            numValue < minValue)
        {
            return new ValidationErrorDto { FieldCode = field.FieldCode, Message = validation.ErrorMessage };
        }
        return null;
    }

    private ValidationErrorDto? ValidateMax(FormFieldDto field, object value, FieldValidationDto validation)
    {
        if (double.TryParse(value.ToString(), out var numValue) && 
            double.TryParse(validation.RuleValue, out var maxValue) && 
            numValue > maxValue)
        {
            return new ValidationErrorDto { FieldCode = field.FieldCode, Message = validation.ErrorMessage };
        }
        return null;
    }

    private ValidationErrorDto? ValidateRange(FormFieldDto field, object value, FieldValidationDto validation)
    {
        if (string.IsNullOrEmpty(validation.RuleValue)) return null;
        
        try
        {
            var range = JsonSerializer.Deserialize<Dictionary<string, double>>(validation.RuleValue);
            if (range == null) return null;

            if (double.TryParse(value.ToString(), out var numValue))
            {
                if (range.ContainsKey("min") && numValue < range["min"])
                {
                    return new ValidationErrorDto { FieldCode = field.FieldCode, Message = validation.ErrorMessage };
                }
                if (range.ContainsKey("max") && numValue > range["max"])
                {
                    return new ValidationErrorDto { FieldCode = field.FieldCode, Message = validation.ErrorMessage };
                }
            }
        }
        catch { }

        return null;
    }

    private ValidationErrorDto? ValidateRegex(FormFieldDto field, object value, FieldValidationDto validation)
    {
        if (string.IsNullOrEmpty(validation.RuleValue)) return null;

        try
        {
            var regex = new System.Text.RegularExpressions.Regex(validation.RuleValue);
            if (!regex.IsMatch(value.ToString() ?? ""))
            {
                return new ValidationErrorDto { FieldCode = field.FieldCode, Message = validation.ErrorMessage };
            }
        }
        catch { }

        return null;
    }

    public async Task<List<FormDataListItemDto>> GetFormDataListAsync(Guid? formVersionPublicId = null, string? objectType = null, string? objectId = null)
    {
        var baseQuery = _context.FormDataValues.AsQueryable();

        // Filter by FormVersion
        int? formVersionId = null;
        if (formVersionPublicId.HasValue)
        {
            formVersionId = await _context.FormVersions
                .Where(v => v.PublicId == formVersionPublicId.Value)
                .Select(v => (int?)v.Id)
                .FirstOrDefaultAsync();
            
            if (formVersionId.HasValue)
            {
                baseQuery = baseQuery.Where(v => v.FormVersionId == formVersionId.Value);
            }
            else
            {
                return new List<FormDataListItemDto>();
            }
        }

        // Filter by ObjectType
        if (!string.IsNullOrEmpty(objectType))
        {
            baseQuery = baseQuery.Where(v => v.ObjectType == objectType);
        }

        // Filter by ObjectId
        if (!string.IsNullOrEmpty(objectId))
        {
            baseQuery = baseQuery.Where(v => v.ObjectId == objectId);
        }

        // Get distinct SubmissionIds with their metadata
        var submissionGroups = await baseQuery
            .GroupBy(v => new
            {
                v.SubmissionId,
                v.FormVersionId,
                v.ObjectId,
                v.ObjectType,
                v.Status,
                v.CreatedDate,
                v.CreatedBy,
                v.ModifiedDate,
                v.ModifiedBy
            })
            .Select(g => new
            {
                SubmissionId = g.Key.SubmissionId,
                FormVersionId = g.Key.FormVersionId,
                ObjectId = g.Key.ObjectId,
                ObjectType = g.Key.ObjectType,
                Status = g.Key.Status,
                CreatedDate = g.Key.CreatedDate,
                CreatedBy = g.Key.CreatedBy,
                ModifiedDate = g.Key.ModifiedDate,
                ModifiedBy = g.Key.ModifiedBy,
                FieldCount = g.Count()
            })
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();

        if (submissionGroups.Count == 0)
        {
            return new List<FormDataListItemDto>();
        }

        // Get all unique FormVersionIds
        var formVersionIds = submissionGroups.Select(s => s.FormVersionId).Distinct().ToList();
        
        // Load FormVersions with Forms
        var formVersions = await _context.FormVersions
            .Include(v => v.Form)
            .Where(v => formVersionIds.Contains(v.Id))
            .ToListAsync();

        var formVersionDict = formVersions.ToDictionary(v => v.Id);

        // Build result
        var result = new List<FormDataListItemDto>();
        
        foreach (var sub in submissionGroups)
        {
            if (!formVersionDict.TryGetValue(sub.FormVersionId, out var formVersion))
            {
                continue; // Skip if FormVersion not found
            }

            result.Add(new FormDataListItemDto
            {
                SubmissionId = sub.SubmissionId,
                FormVersionId = formVersion.PublicId,
                FormVersionName = formVersion.Version,
                FormName = formVersion.Form.Name,
                FormCode = formVersion.Form.Code,
                ObjectId = sub.ObjectId,
                ObjectType = sub.ObjectType,
                CreatedDate = sub.CreatedDate,
                CreatedBy = sub.CreatedBy,
                ModifiedDate = sub.ModifiedDate,
                ModifiedBy = sub.ModifiedBy,
                Status = sub.Status,
                FieldCount = sub.FieldCount
            });
        }

        return result;
    }
}
