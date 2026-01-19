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

    public async Task<FormDataDto?> GetFormDataAsync(Guid id)
    {
        var formData = await _context.FormData
            .Include(fd => fd.FormVersion)
            .FirstOrDefaultAsync(fd => fd.Id == id);

        if (formData == null) return null;

        var dataDict = JsonSerializer.Deserialize<Dictionary<string, object>>(formData.DataJson) 
            ?? new Dictionary<string, object>();

        return new FormDataDto
        {
            Id = formData.Id,
            FormVersionId = formData.FormVersionId,
            ObjectId = formData.ObjectId,
            ObjectType = formData.ObjectType,
            Data = dataDict,
            CreatedDate = formData.CreatedDate,
            CreatedBy = formData.CreatedBy,
            ModifiedDate = formData.ModifiedDate,
            ModifiedBy = formData.ModifiedBy,
            Status = formData.Status
        };
    }

    public async Task<FormDataDto?> GetFormDataByObjectAsync(string objectId, string objectType)
    {
        var formData = await _context.FormData
            .Include(fd => fd.FormVersion)
            .FirstOrDefaultAsync(fd => fd.ObjectId == objectId && fd.ObjectType == objectType);

        if (formData == null) return null;

        return await GetFormDataAsync(formData.Id);
    }

    public async Task<FormDataDto> CreateFormDataAsync(CreateFormDataRequest request)
    {
        // Validate data
        var validationResult = await ValidateFormDataAsync(request.FormVersionId, request.Data);
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.Message))}");
        }

        var formData = new FormData
        {
            FormVersionId = request.FormVersionId,
            ObjectId = request.ObjectId,
            ObjectType = request.ObjectType,
            DataJson = JsonSerializer.Serialize(request.Data),
            CreatedBy = "System", // TODO: Get from current user
            CreatedDate = DateTime.UtcNow,
            Status = 0 // Draft
        };

        _context.FormData.Add(formData);
        await _context.SaveChangesAsync();

        return await GetFormDataAsync(formData.Id) ?? throw new Exception("Failed to retrieve created form data");
    }

    public async Task<FormDataDto> UpdateFormDataAsync(Guid id, CreateFormDataRequest request)
    {
        var formData = await _context.FormData.FindAsync(id);
        if (formData == null) throw new ArgumentException("Form data not found");

        // Validate data
        var validationResult = await ValidateFormDataAsync(request.FormVersionId, request.Data);
        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.Message))}");
        }

        // Save history
        var history = new FormDataHistory
        {
            FormDataId = formData.Id,
            DataJson = formData.DataJson,
            ChangedBy = formData.ModifiedBy ?? formData.CreatedBy,
            ChangedDate = DateTime.UtcNow,
            ChangeReason = "Update"
        };
        _context.FormDataHistory.Add(history);

        // Update data
        formData.DataJson = JsonSerializer.Serialize(request.Data);
        formData.ModifiedBy = "System"; // TODO: Get from current user
        formData.ModifiedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetFormDataAsync(id) ?? throw new Exception("Failed to retrieve updated form data");
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
}
