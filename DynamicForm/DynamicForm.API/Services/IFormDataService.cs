using DynamicForm.API.DTOs;

namespace DynamicForm.API.Services;

public interface IFormDataService
{
    Task<FormDataDto?> GetFormDataAsync(Guid id);
    Task<FormDataDto?> GetFormDataByObjectAsync(string objectId, string objectType);
    Task<FormDataDto> CreateFormDataAsync(CreateFormDataRequest request);
    Task<FormDataDto> UpdateFormDataAsync(Guid id, CreateFormDataRequest request);
    Task<ValidationResultDto> ValidateFormDataAsync(Guid formVersionId, Dictionary<string, object> data);
}
