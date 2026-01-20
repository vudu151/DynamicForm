namespace DynamicForm.Mobile.Models;

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
