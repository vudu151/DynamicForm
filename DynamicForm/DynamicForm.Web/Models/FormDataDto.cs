namespace DynamicForm.Web.Models;

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
