namespace DynamicForm.Web.Models;

public class FormDataListItem
{
    public int SubmissionId { get; set; }
    public Guid FormVersionId { get; set; }
    public string FormVersionName { get; set; } = string.Empty;
    public string FormName { get; set; } = string.Empty;
    public string FormCode { get; set; } = string.Empty;
    public string ObjectId { get; set; } = string.Empty;
    public string ObjectType { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public int Status { get; set; }
    public int FieldCount { get; set; }
}
