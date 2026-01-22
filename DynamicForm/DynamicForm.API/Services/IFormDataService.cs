using DynamicForm.API.DTOs;

namespace DynamicForm.API.Services;

public interface IFormDataService
{
    Task<FormDataDto?> GetFormDataAsync(int submissionId);
    Task<FormDataDto?> GetFormDataByObjectAsync(string objectId, string objectType, Guid formVersionPublicId);
    Task<FormDataDto> CreateFormDataAsync(CreateFormDataRequest request);
    Task<FormDataDto> UpdateFormDataAsync(int submissionId, CreateFormDataRequest request);
    Task<ValidationResultDto> ValidateFormDataAsync(Guid formVersionId, Dictionary<string, object> data);
    Task<List<FormDataListItemDto>> GetFormDataListAsync(Guid? formVersionPublicId = null, string? objectType = null, string? objectId = null);
}
