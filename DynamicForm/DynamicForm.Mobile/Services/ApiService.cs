using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DynamicForm.Mobile.Models;

namespace DynamicForm.Mobile.Services;

public class ApiService
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri(ApiConfig.BaseUrl)
        };

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<FormDto>> GetFormsAsync(CancellationToken ct = default)
    {
        using var resp = await _client.GetAsync("/api/forms", ct);

        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync(ct);
            throw new Exception($"Cannot load forms. Status: {resp.StatusCode}, Error: {error}");
        }

        var json = await resp.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<List<FormDto>>(json, _jsonOptions) ?? new List<FormDto>();
    }

    public async Task<FormMetadataDto?> GetFormMetadataByCodeAsync(string code, CancellationToken ct = default)
    {
        var url = $"/api/forms/code/{Uri.EscapeDataString(code)}/metadata";
        using var resp = await _client.GetAsync(url, ct);

        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync(ct);
            throw new Exception($"Cannot load form metadata. Status: {resp.StatusCode}, Error: {error}");
        }

        var json = await resp.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<FormMetadataDto>(json, _jsonOptions);
    }

    public async Task<ValidationResultDto> ValidateFormDataAsync(Guid formVersionId, Dictionary<string, object> data, CancellationToken ct = default)
    {
        var payload = new
        {
            FormVersionId = formVersionId,
            Data = data
        };

        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var resp = await _client.PostAsync("/api/formdata/validate", content, ct);

        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync(ct);
            throw new Exception($"Validate failed. Status: {resp.StatusCode}, Error: {error}");
        }

        var responseJson = await resp.Content.ReadAsStringAsync(ct);
        return JsonSerializer.Deserialize<ValidationResultDto>(responseJson, _jsonOptions)!;
    }

    public async Task CreateFormDataAsync(CreateFormDataRequest request, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var resp = await _client.PostAsync("/api/formdata", content, ct);

        if (!resp.IsSuccessStatusCode)
        {
            var error = await resp.Content.ReadAsStringAsync(ct);
            throw new Exception($"Submit form failed. Status: {resp.StatusCode}, Error: {error}");
        }
    }
}
