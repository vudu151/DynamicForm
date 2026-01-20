namespace DynamicForm.Mobile.Models;

public class FormMetadataDto
{
    public FormDto Form { get; set; } = null!;
    public FormVersionDto Version { get; set; } = null!;
    public List<FormFieldDto> Fields { get; set; } = new();
}
