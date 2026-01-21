using DynamicForm.API.Data;
using DynamicForm.API.DTOs;
using DynamicForm.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicForm.API.Services;

public class FormService : IFormService
{
    private readonly ApplicationDbContext _context;

    public FormService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<FormVersionDto>> GetVersionsByFormIdAsync(Guid formPublicId)
    {
        // Convert Form PublicId -> Form Id
        var formId = await _context.Forms
            .Where(f => f.PublicId == formPublicId)
            .Select(f => (int?)f.Id)
            .FirstOrDefaultAsync();
        
        if (formId == null) return new List<FormVersionDto>();

        return await _context.FormVersions
            .Where(v => v.FormId == formId.Value)
            .OrderByDescending(v => v.CreatedDate)
            .Select(v => new FormVersionDto
            {
                Id = v.PublicId, // Return PublicId
                FormId = v.Form.PublicId, // Return Form PublicId
                Version = v.Version,
                IsActive = v.IsActive || v.Status == 1, // Published = Active
                CreatedDate = v.CreatedDate,
                CreatedBy = v.CreatedBy,
                ApprovedDate = v.PublishedDate ?? v.ApprovedDate,
                ApprovedBy = v.PublishedBy ?? v.ApprovedBy,
                ChangeLog = v.ChangeLog
            })
            .ToListAsync();
    }

    public async Task<List<FormDto>> GetAllFormsAsync()
    {
        return await _context.Forms
            .Include(f => f.CurrentVersion)
            .Select(f => new FormDto
            {
                Id = f.PublicId, // Return PublicId
                Code = f.Code,
                Name = f.Name,
                Description = f.Description,
                Status = f.Status,
                CurrentVersionId = f.CurrentVersion != null ? f.CurrentVersion.PublicId : null, // Return PublicId
                CreatedDate = f.CreatedDate,
                CreatedBy = f.CreatedBy
            })
            .ToListAsync();
    }

    public async Task<FormDto?> GetFormByCodeAsync(string code)
    {
        var form = await _context.Forms
            .FirstOrDefaultAsync(f => f.Code == code);

        if (form == null) return null;

        return new FormDto
        {
            Id = form.PublicId, // Return PublicId cho API
            Code = form.Code,
            Name = form.Name,
            Description = form.Description,
            Status = form.Status,
            CurrentVersionId = form.CurrentVersion?.PublicId, // Return PublicId
            CreatedDate = form.CreatedDate,
            CreatedBy = form.CreatedBy
        };
    }

    public async Task<FormDto?> GetFormByIdAsync(Guid publicId)
    {
        // Convert PublicId (Guid) -> Id (int)
        var form = await _context.Forms
            .FirstOrDefaultAsync(f => f.PublicId == publicId);
        if (form == null) return null;

        return new FormDto
        {
            Id = form.PublicId, // Return PublicId cho API
            Code = form.Code,
            Name = form.Name,
            Description = form.Description,
            Status = form.Status,
            CurrentVersionId = form.CurrentVersion?.PublicId, // Return PublicId
            CreatedDate = form.CreatedDate,
            CreatedBy = form.CreatedBy
        };
    }

    public async Task<FormMetadataDto?> GetFormMetadataAsync(string code)
    {
        var form = await _context.Forms
            .Include(f => f.CurrentVersion)
            .FirstOrDefaultAsync(f => f.Code == code);

        if (form?.CurrentVersion == null) return null;

        return await GetFormMetadataByVersionIdAsync(form.CurrentVersion.PublicId);
    }

    public async Task<FormMetadataDto?> GetFormMetadataByVersionIdAsync(Guid versionPublicId)
    {
        // Convert Version PublicId -> Version Id
        var version = await _context.FormVersions
            .Include(v => v.Form)
            .Include(v => v.Fields)
                .ThenInclude(f => f.Validations)
            .Include(v => v.Fields)
                .ThenInclude(f => f.Conditions)
            .Include(v => v.Fields)
                .ThenInclude(f => f.Options)
            .FirstOrDefaultAsync(v => v.PublicId == versionPublicId);

        if (version == null) return null;

        return new FormMetadataDto
        {
            Form = new FormDto
            {
                Id = version.Form.PublicId, // Return PublicId
                Code = version.Form.Code,
                Name = version.Form.Name,
                Description = version.Form.Description,
                Status = version.Form.Status,
                CurrentVersionId = version.Form.CurrentVersion?.PublicId, // Return PublicId
                CreatedDate = version.Form.CreatedDate,
                CreatedBy = version.Form.CreatedBy
            },
            Version = new FormVersionDto
            {
                Id = version.PublicId, // Return PublicId
                FormId = version.Form.PublicId, // Return Form PublicId
                Version = version.Version,
                IsActive = version.IsActive || version.Status == 1, // Published = Active
                CreatedDate = version.CreatedDate,
                CreatedBy = version.CreatedBy,
                ApprovedDate = version.PublishedDate ?? version.ApprovedDate,
                ApprovedBy = version.PublishedBy ?? version.ApprovedBy,
                ChangeLog = version.ChangeLog
            },
            Fields = version.Fields
                .OrderBy(f => f.DisplayOrder)
                .Select(f => new FormFieldDto
                {
                    Id = f.PublicId, // Return PublicId
                    FormVersionId = version.PublicId, // Return Version PublicId
                    FieldCode = f.FieldCode,
                    FieldType = f.FieldType,
                    Label = f.Label,
                    DisplayOrder = f.DisplayOrder,
                    IsRequired = f.IsRequired,
                    IsVisible = f.IsVisible,
                    DefaultValue = f.DefaultValue,
                    Placeholder = f.Placeholder,
                    HelpText = f.HelpText,
                    CssClass = f.CssClass,
                    PropertiesJson = f.PropertiesJson,
                    ParentFieldId = f.ParentField?.PublicId, // Return ParentField PublicId
                    MinOccurs = f.MinOccurs,
                    MaxOccurs = f.MaxOccurs,
                    SectionCode = f.SectionCode,
                    Validations = f.Validations
                        .Where(v => v.IsActive)
                        .OrderBy(v => v.Priority)
                        .Select(v => new FieldValidationDto
                        {
                            Id = v.PublicId, // Return PublicId
                            FieldId = f.PublicId, // Return Field PublicId
                            RuleType = v.RuleType,
                            RuleValue = v.RuleValue,
                            ErrorMessage = v.ErrorMessage,
                            Priority = v.Priority,
                            IsActive = v.IsActive
                        }).ToList(),
                    Conditions = f.Conditions
                        .OrderBy(c => c.Priority)
                        .Select(c => new FieldConditionDto
                        {
                            Id = c.PublicId, // Return PublicId
                            FieldId = f.PublicId, // Return Field PublicId
                            ConditionType = c.ConditionType,
                            Expression = c.Expression,
                            ActionsJson = c.ActionsJson,
                            Priority = c.Priority
                        }).ToList(),
                    Options = f.Options
                        .OrderBy(o => o.DisplayOrder)
                        .Select(o => new FieldOptionDto
                        {
                            Id = o.PublicId, // Return PublicId
                            FieldId = f.PublicId, // Return Field PublicId
                            Value = o.Value,
                            Label = o.Label,
                            DisplayOrder = o.DisplayOrder,
                            IsDefault = o.IsDefault
                        }).ToList()
                }).ToList()
        };
    }

    public async Task<FormMetadataDto?> UpdateFormMetadataByVersionIdAsync(Guid versionPublicId, UpdateFormMetadataRequest request)
    {
        // Convert Version PublicId -> Version Id
        var version = await _context.FormVersions
            .Include(v => v.Fields)
                .ThenInclude(f => f.Options)
            .Include(v => v.Fields)
                .ThenInclude(f => f.Validations)
            .Include(v => v.Fields)
                .ThenInclude(f => f.Conditions)
            .FirstOrDefaultAsync(v => v.PublicId == versionPublicId);

        if (version == null) return null;

        version.ChangeLog = request.ChangeLog;

        // Remove deleted fields (map PublicId -> Id)
        var incomingFieldPublicIds = request.Fields
            .Where(f => f.Id != Guid.Empty)
            .Select(f => f.Id) // f.Id là PublicId trong DTO
            .ToHashSet();

        var fieldsToRemove = version.Fields
            .Where(f => !incomingFieldPublicIds.Contains(f.PublicId))
            .ToList();

        if (fieldsToRemove.Count > 0)
        {
            _context.FormFields.RemoveRange(fieldsToRemove);
        }

        // Upsert fields (map PublicId -> Id)
        var existingFieldsByPublicId = version.Fields.ToDictionary(f => f.PublicId, f => f);
        foreach (var f in request.Fields)
        {
            FormField entity;
            if (f.Id != Guid.Empty && existingFieldsByPublicId.TryGetValue(f.Id, out var existingEntity))
            {
                // f.Id là PublicId trong DTO
                entity = existingEntity;
            }
            else
            {
                // Tạo field mới với PublicId mới
                entity = new FormField
                {
                    PublicId = f.Id == Guid.Empty ? Guid.NewGuid() : f.Id,
                    FormVersionId = version.Id // Dùng version.Id (int)
                };
                _context.FormFields.Add(entity);
            }

            entity.FieldCode = f.FieldCode;
            entity.FieldType = f.FieldType;
            entity.Label = f.Label;
            entity.DisplayOrder = f.DisplayOrder;
            entity.IsRequired = f.IsRequired;
            entity.IsVisible = f.IsVisible;
            entity.DefaultValue = f.DefaultValue;
            entity.Placeholder = f.Placeholder;
            entity.HelpText = f.HelpText;
            entity.CssClass = f.CssClass;
            entity.PropertiesJson = f.PropertiesJson;
            // Map ParentFieldId (PublicId -> Id)
            if (f.ParentFieldId.HasValue)
            {
                var parentFieldId = await _context.FormFields
                    .Where(pf => pf.PublicId == f.ParentFieldId.Value)
                    .Select(pf => (int?)pf.Id)
                    .FirstOrDefaultAsync();
                entity.ParentFieldId = parentFieldId;
            }
            else
            {
                entity.ParentFieldId = null;
            }
            entity.MinOccurs = f.MinOccurs;
            entity.MaxOccurs = f.MaxOccurs;

            // Options (map PublicId -> Id)
            var incomingOptionsByPublicId = f.Options
                .Where(o => o.Id != Guid.Empty)
                .ToDictionary(o => o.Id, o => o); // o.Id là PublicId trong DTO
            var optionsToRemove = entity.Options
                .Where(o => !incomingOptionsByPublicId.ContainsKey(o.PublicId))
                .ToList();
            if (optionsToRemove.Count > 0)
            {
                _context.FieldOptions.RemoveRange(optionsToRemove);
            }
            foreach (var o in f.Options)
            {
                var optIsNew = o.Id == Guid.Empty || entity.Options.All(eo => eo.PublicId != o.Id);
                FieldOption optEntity;
                if (optIsNew)
                {
                    optEntity = new FieldOption
                    {
                        PublicId = o.Id == Guid.Empty ? Guid.NewGuid() : o.Id,
                        FieldId = entity.Id // Dùng entity.Id (int)
                    };
                    entity.Options.Add(optEntity);
                }
                else
                {
                    optEntity = entity.Options.First(eo => eo.PublicId == o.Id);
                }
                optEntity.Value = o.Value;
                optEntity.Label = o.Label;
                optEntity.DisplayOrder = o.DisplayOrder;
                optEntity.IsDefault = o.IsDefault;
            }

            // Validations (map PublicId -> Id)
            var incomingValidationsByPublicId = f.Validations
                .Where(v => v.Id != Guid.Empty)
                .ToDictionary(v => v.Id, v => v); // v.Id là PublicId trong DTO
            var validationsToRemove = entity.Validations
                .Where(v => !incomingValidationsByPublicId.ContainsKey(v.PublicId))
                .ToList();
            if (validationsToRemove.Count > 0)
            {
                _context.FieldValidations.RemoveRange(validationsToRemove);
            }
            foreach (var v in f.Validations)
            {
                var valIsNew = v.Id == Guid.Empty || entity.Validations.All(ev => ev.PublicId != v.Id);
                FieldValidation valEntity;
                if (valIsNew)
                {
                    valEntity = new FieldValidation
                    {
                        PublicId = v.Id == Guid.Empty ? Guid.NewGuid() : v.Id,
                        FieldId = entity.Id // Dùng entity.Id (int)
                    };
                    entity.Validations.Add(valEntity);
                }
                else
                {
                    valEntity = entity.Validations.First(ev => ev.PublicId == v.Id);
                }

                valEntity.RuleType = v.RuleType;
                valEntity.RuleValue = v.RuleValue;
                valEntity.ErrorMessage = v.ErrorMessage;
                valEntity.Priority = v.Priority;
                valEntity.IsActive = v.IsActive;
            }

            // Conditions (map PublicId -> Id)
            var incomingConditionsByPublicId = f.Conditions
                .Where(c => c.Id != Guid.Empty)
                .ToDictionary(c => c.Id, c => c); // c.Id là PublicId trong DTO
            var conditionsToRemove = entity.Conditions
                .Where(c => !incomingConditionsByPublicId.ContainsKey(c.PublicId))
                .ToList();
            if (conditionsToRemove.Count > 0)
            {
                _context.FieldConditions.RemoveRange(conditionsToRemove);
            }
            foreach (var c in f.Conditions)
            {
                var condIsNew = c.Id == Guid.Empty || entity.Conditions.All(ec => ec.PublicId != c.Id);
                FieldCondition condEntity;
                if (condIsNew)
                {
                    condEntity = new FieldCondition
                    {
                        PublicId = c.Id == Guid.Empty ? Guid.NewGuid() : c.Id,
                        FieldId = entity.Id // Dùng entity.Id (int)
                    };
                    entity.Conditions.Add(condEntity);
                }
                else
                {
                    condEntity = entity.Conditions.First(ec => ec.PublicId == c.Id);
                }

                condEntity.ConditionType = c.ConditionType;
                condEntity.Expression = c.Expression;
                condEntity.ActionsJson = c.ActionsJson;
                condEntity.Priority = c.Priority;
            }
        }

        await _context.SaveChangesAsync();
        return await GetFormMetadataByVersionIdAsync(version.PublicId);
    }

    public async Task<FormDto> CreateFormAsync(FormDto formDto)
    {
        var code = formDto.Code?.Trim();
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new InvalidOperationException("Form code is required");
        }

        // Friendly validation instead of DB unique constraint exception
        var exists = await _context.Forms.AnyAsync(f => f.Code == code);
        if (exists)
        {
            throw new InvalidOperationException($"Form code already exists: {code}");
        }

        var form = new Form
        {
            Code = code,
            Name = formDto.Name,
            Description = formDto.Description,
            Status = formDto.Status,
            CreatedBy = formDto.CreatedBy,
            CreatedDate = DateTime.UtcNow
        };

        _context.Forms.Add(form);
        await _context.SaveChangesAsync();

        formDto.Id = form.PublicId; // Return PublicId
        formDto.CreatedDate = form.CreatedDate;
        return formDto;
    }

    public async Task<FormVersionDto> CreateVersionAsync(Guid formPublicId, FormVersionDto versionDto)
    {
        // Convert Form PublicId -> Form Id
        var form = await _context.Forms
            .FirstOrDefaultAsync(f => f.PublicId == formPublicId);
        if (form == null) throw new ArgumentException("Form not found");

        var versionText = versionDto.Version?.Trim();
        if (string.IsNullOrWhiteSpace(versionText))
        {
            throw new InvalidOperationException("Version is required");
        }

        var exists = await _context.FormVersions.AnyAsync(v => v.FormId == form.Id && v.Version == versionText);
        if (exists)
        {
            throw new InvalidOperationException($"Version already exists: {versionText}");
        }

        var version = new FormVersion
        {
            FormId = form.Id, // Dùng form.Id (int)
            Version = versionText,
            IsActive = false,
            CreatedBy = versionDto.CreatedBy,
            CreatedDate = DateTime.UtcNow,
            ChangeLog = versionDto.ChangeLog
        };

        _context.FormVersions.Add(version);
        await _context.SaveChangesAsync();

        versionDto.Id = version.PublicId; // Return PublicId
        versionDto.CreatedDate = version.CreatedDate;
        return versionDto;
    }

    public async Task<bool> ActivateVersionAsync(Guid versionPublicId)
    {
        // Convert Version PublicId -> Version Id
        var version = await _context.FormVersions
            .Include(v => v.Form)
            .FirstOrDefaultAsync(v => v.PublicId == versionPublicId);

        if (version == null) return false;

        // Publish version: Set Status = Published (1)
        // Archive all other versions of this form
        var otherVersions = await _context.FormVersions
            .Where(v => v.FormId == version.FormId && v.Id != version.Id) // Dùng version.Id (int)
            .ToListAsync();

        foreach (var v in otherVersions)
        {
            if (v.Status == 1) // If was Published, archive it
            {
                v.Status = 2; // Archived
            }
            v.IsActive = false; // Keep for backward compatibility
        }

        // Publish this version
        version.Status = 1; // Published
        version.IsActive = true; // Keep for backward compatibility
        version.Form.CurrentVersionId = version.Id; // Dùng version.Id (int)
        version.Form.Status = 1; // Mark form as Active when a version is published
        version.PublishedDate = DateTime.UtcNow;
        version.PublishedBy = "System"; // TODO: Get from current user
        // Keep ApprovedDate for backward compatibility
        version.ApprovedDate = version.PublishedDate;
        version.ApprovedBy = version.PublishedBy;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateFormAsync(Guid formPublicId)
    {
        // Convert Form PublicId -> Form Id
        var form = await _context.Forms
            .Include(f => f.Versions)
            .FirstOrDefaultAsync(f => f.PublicId == formPublicId);

        if (form == null) return false;

        // Deactivate all versions and detach current version
        foreach (var v in form.Versions)
        {
            v.IsActive = false;
        }

        form.CurrentVersionId = null;
        form.Status = 2; // Inactive
        form.ModifiedDate = DateTime.UtcNow;
        form.ModifiedBy = "System"; // TODO: current user

        await _context.SaveChangesAsync();
        return true;
    }
}
