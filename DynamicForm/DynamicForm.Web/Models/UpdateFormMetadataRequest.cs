namespace DynamicForm.Web.Models;

public class UpdateFormMetadataRequest
{
    public string? ChangeLog { get; set; }
    public List<FormFieldInfo> Fields { get; set; } = new();
}

