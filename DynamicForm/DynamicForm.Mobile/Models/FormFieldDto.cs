namespace DynamicForm.Mobile.Models;

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
