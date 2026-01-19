namespace DynamicForm.API.DTOs;

public class FormDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Status { get; set; }
    public Guid? CurrentVersionId { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class FormVersionDto
{
    public Guid Id { get; set; }
    public Guid FormId { get; set; }
    public string Version { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovedBy { get; set; }
    public string? ChangeLog { get; set; }
}

public class FormFieldDto
{
    public Guid Id { get; set; }
    public Guid FormVersionId { get; set; }
    public string FieldCode { get; set; } = string.Empty;
    public int FieldType { get; set; }
    public string Label { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; }
    public bool IsVisible { get; set; }
    public string? DefaultValue { get; set; }
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public string? CssClass { get; set; }
    public string? PropertiesJson { get; set; }
    public Guid? ParentFieldId { get; set; }
    public int? MinOccurs { get; set; }
    public int? MaxOccurs { get; set; }
    public List<FieldValidationDto> Validations { get; set; } = new();
    public List<FieldConditionDto> Conditions { get; set; } = new();
    public List<FieldOptionDto> Options { get; set; } = new();
}

public class FieldValidationDto
{
    public Guid Id { get; set; }
    public Guid FieldId { get; set; }
    public int RuleType { get; set; }
    public string? RuleValue { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public int Priority { get; set; }
    public bool IsActive { get; set; }
}

public class FieldConditionDto
{
    public Guid Id { get; set; }
    public Guid FieldId { get; set; }
    public int ConditionType { get; set; }
    public string Expression { get; set; } = string.Empty;
    public string? ActionsJson { get; set; }
    public int Priority { get; set; }
}

public class FieldOptionDto
{
    public Guid Id { get; set; }
    public Guid FieldId { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsDefault { get; set; }
}

public class FormMetadataDto
{
    public FormDto Form { get; set; } = null!;
    public FormVersionDto Version { get; set; } = null!;
    public List<FormFieldDto> Fields { get; set; } = new();
}

public class FormDataDto
{
    public Guid Id { get; set; }
    public Guid FormVersionId { get; set; }
    public string ObjectId { get; set; } = string.Empty;
    public string ObjectType { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public int Status { get; set; }
}

public class CreateFormDataRequest
{
    public Guid FormVersionId { get; set; }
    public string ObjectId { get; set; } = string.Empty;
    public string ObjectType { get; set; } = string.Empty;
    public Dictionary<string, object> Data { get; set; } = new();
}

public class ValidationResultDto
{
    public bool IsValid { get; set; }
    public List<ValidationErrorDto> Errors { get; set; } = new();
}

public class ValidationErrorDto
{
    public string FieldCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
