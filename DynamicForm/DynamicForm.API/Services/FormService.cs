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

    public async Task<List<FormVersionDto>> GetVersionsByFormIdAsync(Guid formId)
    {
        return await _context.FormVersions
            .Where(v => v.FormId == formId)
            .OrderByDescending(v => v.CreatedDate)
            .Select(v => new FormVersionDto
            {
                Id = v.Id,
                FormId = v.FormId,
                Version = v.Version,
                IsActive = v.IsActive,
                CreatedDate = v.CreatedDate,
                CreatedBy = v.CreatedBy,
                ApprovedDate = v.ApprovedDate,
                ApprovedBy = v.ApprovedBy,
                ChangeLog = v.ChangeLog
            })
            .ToListAsync();
    }

    public async Task<List<FormDto>> GetAllFormsAsync()
    {
        return await _context.Forms
            .Select(f => new FormDto
            {
                Id = f.Id,
                Code = f.Code,
                Name = f.Name,
                Description = f.Description,
                Status = f.Status,
                CurrentVersionId = f.CurrentVersionId,
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
            Id = form.Id,
            Code = form.Code,
            Name = form.Name,
            Description = form.Description,
            Status = form.Status,
            CurrentVersionId = form.CurrentVersionId,
            CreatedDate = form.CreatedDate,
            CreatedBy = form.CreatedBy
        };
    }

    public async Task<FormDto?> GetFormByIdAsync(Guid id)
    {
        var form = await _context.Forms.FindAsync(id);
        if (form == null) return null;

        return new FormDto
        {
            Id = form.Id,
            Code = form.Code,
            Name = form.Name,
            Description = form.Description,
            Status = form.Status,
            CurrentVersionId = form.CurrentVersionId,
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

        return await GetFormMetadataByVersionIdAsync(form.CurrentVersion.Id);
    }

    public async Task<FormMetadataDto?> GetFormMetadataByVersionIdAsync(Guid versionId)
    {
        var version = await _context.FormVersions
            .Include(v => v.Form)
            .Include(v => v.Fields)
                .ThenInclude(f => f.Validations)
            .Include(v => v.Fields)
                .ThenInclude(f => f.Conditions)
            .Include(v => v.Fields)
                .ThenInclude(f => f.Options)
            .FirstOrDefaultAsync(v => v.Id == versionId);

        if (version == null) return null;

        return new FormMetadataDto
        {
            Form = new FormDto
            {
                Id = version.Form.Id,
                Code = version.Form.Code,
                Name = version.Form.Name,
                Description = version.Form.Description,
                Status = version.Form.Status,
                CurrentVersionId = version.Form.CurrentVersionId,
                CreatedDate = version.Form.CreatedDate,
                CreatedBy = version.Form.CreatedBy
            },
            Version = new FormVersionDto
            {
                Id = version.Id,
                FormId = version.FormId,
                Version = version.Version,
                IsActive = version.IsActive,
                CreatedDate = version.CreatedDate,
                CreatedBy = version.CreatedBy,
                ApprovedDate = version.ApprovedDate,
                ApprovedBy = version.ApprovedBy,
                ChangeLog = version.ChangeLog
            },
            Fields = version.Fields
                .OrderBy(f => f.DisplayOrder)
                .Select(f => new FormFieldDto
                {
                    Id = f.Id,
                    FormVersionId = f.FormVersionId,
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
                    ParentFieldId = f.ParentFieldId,
                    MinOccurs = f.MinOccurs,
                    MaxOccurs = f.MaxOccurs,
                    Validations = f.Validations
                        .Where(v => v.IsActive)
                        .OrderBy(v => v.Priority)
                        .Select(v => new FieldValidationDto
                        {
                            Id = v.Id,
                            FieldId = v.FieldId,
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
                            Id = c.Id,
                            FieldId = c.FieldId,
                            ConditionType = c.ConditionType,
                            Expression = c.Expression,
                            ActionsJson = c.ActionsJson,
                            Priority = c.Priority
                        }).ToList(),
                    Options = f.Options
                        .OrderBy(o => o.DisplayOrder)
                        .Select(o => new FieldOptionDto
                        {
                            Id = o.Id,
                            FieldId = o.FieldId,
                            Value = o.Value,
                            Label = o.Label,
                            DisplayOrder = o.DisplayOrder,
                            IsDefault = o.IsDefault
                        }).ToList()
                }).ToList()
        };
    }

    public async Task<FormMetadataDto?> UpdateFormMetadataByVersionIdAsync(Guid versionId, UpdateFormMetadataRequest request)
    {
        var version = await _context.FormVersions
            .Include(v => v.Fields)
                .ThenInclude(f => f.Options)
            .Include(v => v.Fields)
                .ThenInclude(f => f.Validations)
            .Include(v => v.Fields)
                .ThenInclude(f => f.Conditions)
            .FirstOrDefaultAsync(v => v.Id == versionId);

        if (version == null) return null;

        version.ChangeLog = request.ChangeLog;

        // Remove deleted fields
        var incomingFieldIds = request.Fields
            .Where(f => f.Id != Guid.Empty)
            .Select(f => f.Id)
            .ToHashSet();

        var fieldsToRemove = version.Fields
            .Where(f => !incomingFieldIds.Contains(f.Id))
            .ToList();

        if (fieldsToRemove.Count > 0)
        {
            _context.FormFields.RemoveRange(fieldsToRemove);
        }

        // Upsert fields
        var existingFieldsById = version.Fields.ToDictionary(f => f.Id, f => f);
        foreach (var f in request.Fields)
        {
            FormField entity;
            if (f.Id != Guid.Empty && existingFieldsById.TryGetValue(f.Id, out var existingEntity))
            {
                entity = existingEntity;
            }
            else
            {
                entity = new FormField
                {
                    Id = f.Id == Guid.Empty ? Guid.NewGuid() : f.Id,
                    FormVersionId = versionId
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
            entity.ParentFieldId = f.ParentFieldId;
            entity.MinOccurs = f.MinOccurs;
            entity.MaxOccurs = f.MaxOccurs;

            // Options
            var incomingOptionsById = f.Options
                .Where(o => o.Id != Guid.Empty)
                .ToDictionary(o => o.Id, o => o);
            var optionsToRemove = entity.Options
                .Where(o => !incomingOptionsById.ContainsKey(o.Id))
                .ToList();
            if (optionsToRemove.Count > 0)
            {
                _context.FieldOptions.RemoveRange(optionsToRemove);
            }
            foreach (var o in f.Options)
            {
                var optIsNew = o.Id == Guid.Empty || entity.Options.All(eo => eo.Id != o.Id);
                FieldOption optEntity;
                if (optIsNew)
                {
                    optEntity = new FieldOption
                    {
                        Id = o.Id == Guid.Empty ? Guid.NewGuid() : o.Id,
                        FieldId = entity.Id
                    };
                    entity.Options.Add(optEntity);
                }
                else
                {
                    optEntity = entity.Options.First(eo => eo.Id == o.Id);
                }
                optEntity.Value = o.Value;
                optEntity.Label = o.Label;
                optEntity.DisplayOrder = o.DisplayOrder;
                optEntity.IsDefault = o.IsDefault;
            }

            // Validations
            var incomingValidationsById = f.Validations
                .Where(v => v.Id != Guid.Empty)
                .ToDictionary(v => v.Id, v => v);
            var validationsToRemove = entity.Validations
                .Where(v => !incomingValidationsById.ContainsKey(v.Id))
                .ToList();
            if (validationsToRemove.Count > 0)
            {
                _context.FieldValidations.RemoveRange(validationsToRemove);
            }
            foreach (var v in f.Validations)
            {
                var valIsNew = v.Id == Guid.Empty || entity.Validations.All(ev => ev.Id != v.Id);
                FieldValidation valEntity;
                if (valIsNew)
                {
                    valEntity = new FieldValidation
                    {
                        Id = v.Id == Guid.Empty ? Guid.NewGuid() : v.Id,
                        FieldId = entity.Id
                    };
                    entity.Validations.Add(valEntity);
                }
                else
                {
                    valEntity = entity.Validations.First(ev => ev.Id == v.Id);
                }

                valEntity.RuleType = v.RuleType;
                valEntity.RuleValue = v.RuleValue;
                valEntity.ErrorMessage = v.ErrorMessage;
                valEntity.Priority = v.Priority;
                valEntity.IsActive = v.IsActive;
            }

            // Conditions
            var incomingConditionsById = f.Conditions
                .Where(c => c.Id != Guid.Empty)
                .ToDictionary(c => c.Id, c => c);
            var conditionsToRemove = entity.Conditions
                .Where(c => !incomingConditionsById.ContainsKey(c.Id))
                .ToList();
            if (conditionsToRemove.Count > 0)
            {
                _context.FieldConditions.RemoveRange(conditionsToRemove);
            }
            foreach (var c in f.Conditions)
            {
                var condIsNew = c.Id == Guid.Empty || entity.Conditions.All(ec => ec.Id != c.Id);
                FieldCondition condEntity;
                if (condIsNew)
                {
                    condEntity = new FieldCondition
                    {
                        Id = c.Id == Guid.Empty ? Guid.NewGuid() : c.Id,
                        FieldId = entity.Id
                    };
                    entity.Conditions.Add(condEntity);
                }
                else
                {
                    condEntity = entity.Conditions.First(ec => ec.Id == c.Id);
                }

                condEntity.ConditionType = c.ConditionType;
                condEntity.Expression = c.Expression;
                condEntity.ActionsJson = c.ActionsJson;
                condEntity.Priority = c.Priority;
            }
        }

        await _context.SaveChangesAsync();
        return await GetFormMetadataByVersionIdAsync(versionId);
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

        formDto.Id = form.Id;
        formDto.CreatedDate = form.CreatedDate;
        return formDto;
    }

    public async Task<FormVersionDto> CreateVersionAsync(Guid formId, FormVersionDto versionDto)
    {
        var form = await _context.Forms.FindAsync(formId);
        if (form == null) throw new ArgumentException("Form not found");

        var versionText = versionDto.Version?.Trim();
        if (string.IsNullOrWhiteSpace(versionText))
        {
            throw new InvalidOperationException("Version is required");
        }

        var exists = await _context.FormVersions.AnyAsync(v => v.FormId == formId && v.Version == versionText);
        if (exists)
        {
            throw new InvalidOperationException($"Version already exists: {versionText}");
        }

        var version = new FormVersion
        {
            FormId = formId,
            Version = versionText,
            IsActive = false,
            CreatedBy = versionDto.CreatedBy,
            CreatedDate = DateTime.UtcNow,
            ChangeLog = versionDto.ChangeLog
        };

        _context.FormVersions.Add(version);
        await _context.SaveChangesAsync();

        versionDto.Id = version.Id;
        versionDto.CreatedDate = version.CreatedDate;
        return versionDto;
    }

    public async Task<bool> ActivateVersionAsync(Guid versionId)
    {
        var version = await _context.FormVersions
            .Include(v => v.Form)
            .FirstOrDefaultAsync(v => v.Id == versionId);

        if (version == null) return false;

        // Deactivate all other versions of this form
        var otherVersions = await _context.FormVersions
            .Where(v => v.FormId == version.FormId && v.Id != versionId)
            .ToListAsync();

        foreach (var v in otherVersions)
        {
            v.IsActive = false;
        }

        version.IsActive = true;
        version.Form.CurrentVersionId = version.Id;
        version.Form.Status = 1; // Mark form as Active when a version is activated
        version.ApprovedDate = DateTime.UtcNow;
        version.ApprovedBy = "System"; // TODO: Get from current user

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateFormAsync(Guid formId)
    {
        var form = await _context.Forms
            .Include(f => f.Versions)
            .FirstOrDefaultAsync(f => f.Id == formId);

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
