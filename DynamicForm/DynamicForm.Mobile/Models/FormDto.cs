namespace DynamicForm.Mobile.Models;

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
