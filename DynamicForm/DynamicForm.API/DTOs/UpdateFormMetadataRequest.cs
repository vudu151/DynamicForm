namespace DynamicForm.API.DTOs;

public class UpdateFormMetadataRequest
{
    public string? ChangeLog { get; set; }
    public List<FormFieldDto> Fields { get; set; } = new();
}

