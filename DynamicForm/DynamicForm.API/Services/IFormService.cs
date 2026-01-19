using DynamicForm.API.DTOs;

namespace DynamicForm.API.Services;

public interface IFormService
{
    Task<List<FormDto>> GetAllFormsAsync();
    Task<FormDto?> GetFormByCodeAsync(string code);
    Task<FormDto?> GetFormByIdAsync(Guid id);
    Task<List<FormVersionDto>> GetVersionsByFormIdAsync(Guid formId);
    Task<FormMetadataDto?> GetFormMetadataAsync(string code);
    Task<FormMetadataDto?> GetFormMetadataByVersionIdAsync(Guid versionId);
    Task<FormMetadataDto?> UpdateFormMetadataByVersionIdAsync(Guid versionId, UpdateFormMetadataRequest request);
    Task<FormDto> CreateFormAsync(FormDto formDto);
    Task<FormVersionDto> CreateVersionAsync(Guid formId, FormVersionDto versionDto);
    Task<bool> ActivateVersionAsync(Guid versionId);
    Task<bool> DeactivateFormAsync(Guid formId);
}
