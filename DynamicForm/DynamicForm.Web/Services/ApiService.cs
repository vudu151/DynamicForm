using System.Text;
using System.Text.Json;

namespace DynamicForm.Web.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;

    /// <summary>
    /// Last error details from the most recent API call (per-request scoped service).
    /// Useful for debugging / surfacing errors to UI.
    /// </summary>
    public string? LastError { get; private set; }

    public ApiService(IHttpClientFactory httpClientFactory, ILogger<ApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        LastError = null;
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                LastError = $"GET {endpoint} -> {(int)response.StatusCode} {response.ReasonPhrase}\n{json}";
                _logger.LogError("API error: {Details}", LastError);
                return default;
            }
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling API: {Endpoint}", endpoint);
            LastError = ex.ToString();
            return default;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        LastError = null;
        try
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content);
            var responseJson = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                LastError = $"POST {endpoint} -> {(int)response.StatusCode} {response.ReasonPhrase}\nRequest:\n{json}\n\nResponse:\n{responseJson}";
                _logger.LogError("API error: {Details}", LastError);
                return default;
            }
            return JsonSerializer.Deserialize<T>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling API: {Endpoint}", endpoint);
            LastError = ex.ToString();
            return default;
        }
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        LastError = null;
        try
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(endpoint, content);
            var responseJson = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                LastError = $"PUT {endpoint} -> {(int)response.StatusCode} {response.ReasonPhrase}\nRequest:\n{json}\n\nResponse:\n{responseJson}";
                _logger.LogError("API error: {Details}", LastError);
                return default;
            }
            return JsonSerializer.Deserialize<T>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling API: {Endpoint}", endpoint);
            LastError = ex.ToString();
            return default;
        }
    }
}
