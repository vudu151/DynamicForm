namespace DynamicForm.Web.Models;

public class FormMetadata
{
    public FormInfo Form { get; set; } = null!;
    public FormVersionInfo Version { get; set; } = null!;
    public List<FormFieldInfo> Fields { get; set; } = new();
}

public class FormInfo
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Status { get; set; }
    public Guid? CurrentVersionId { get; set; }
}

public class FormVersionInfo
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

public class FormFieldInfo
{
    public Guid Id { get; set; }
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
    public List<FieldValidationInfo> Validations { get; set; } = new();
    public List<FieldConditionInfo> Conditions { get; set; } = new();
    public List<FieldOptionInfo> Options { get; set; } = new();
}

public class FieldValidationInfo
{
    public Guid Id { get; set; }
    public Guid FieldId { get; set; }
    public int RuleType { get; set; }
    public string? RuleValue { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;
}

public class FieldOptionInfo
{
    public Guid Id { get; set; }
    public Guid FieldId { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsDefault { get; set; }
}

public class FieldConditionInfo
{
    public Guid Id { get; set; }
    public Guid FieldId { get; set; }
    public int ConditionType { get; set; }
    public string Expression { get; set; } = string.Empty;
    public string? ActionsJson { get; set; }
    public int Priority { get; set; }
}
