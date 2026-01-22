using DynamicForm.Web.Models;
using DynamicForm.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace DynamicForm.Web.Pages.Forms;

public class FillModel : PageModel
{
    private readonly ApiService _apiService;
    private readonly ILogger<FillModel> _logger;

    public FillModel(ApiService apiService, ILogger<FillModel> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public FormMetadata? Metadata { get; set; }

    // IMPORTANT: Binding from HTML form posts works reliably with string values.
    // Dictionary<string, object> often binds as nulls for values.
    [BindProperty]
    public Dictionary<string, string?> FormData { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Code { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? SubmissionId { get; set; }

    [BindProperty]
    public Guid FormVersionId { get; set; }

    public string GetFieldContainerCss(FormFieldInfo field)
    {
        // Priority:
        // 1) field.CssClass (full control from metadata)
        // 2) field.PropertiesJson: {"containerClass":"col-md-12"} or {"colSpan":12}
        // 3) default: 2 columns

        if (!string.IsNullOrWhiteSpace(field.CssClass))
        {
            return $"{field.CssClass} mb-3";
        }

        if (!string.IsNullOrWhiteSpace(field.PropertiesJson))
        {
            try
            {
                using var doc = JsonDocument.Parse(field.PropertiesJson);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("containerClass", out var containerClassEl) &&
                        containerClassEl.ValueKind == JsonValueKind.String)
                    {
                        var containerClass = containerClassEl.GetString();
                        if (!string.IsNullOrWhiteSpace(containerClass))
                        {
                            return $"{containerClass} mb-3";
                        }
                    }

                    if (root.TryGetProperty("colSpan", out var colSpanEl) &&
                        colSpanEl.ValueKind == JsonValueKind.Number &&
                        colSpanEl.TryGetInt32(out var colSpan) &&
                        colSpan is >= 1 and <= 12)
                    {
                        return $"col-md-{colSpan} mb-3";
                    }
                }
            }
            catch
            {
                // Ignore invalid JSON; fall back to default.
            }
        }

        return "col-md-6 mb-3";
    }

    public async Task<IActionResult> OnGetAsync(string? code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return RedirectToPage("/Forms/Index");
        }

        Code = code;

        try
        {
            var metadata = await _apiService.GetAsync<FormMetadata>($"/api/forms/code/{code}/metadata");
            if (metadata == null)
            {
                TempData["Error"] = "Không tìm thấy form";
                return RedirectToPage("/Forms/Index");
            }

            Metadata = metadata;
            FormVersionId = metadata.Version.Id;

            // Load existing data if SubmissionId is provided
            if (SubmissionId.HasValue)
            {
                try
                {
                    var existingData = await _apiService.GetAsync<FormDataDto>($"/api/formdata/{SubmissionId.Value}");
                    if (existingData != null && existingData.Data != null)
                    {
                        foreach (var kvp in existingData.Data)
                        {
                            FormData[kvp.Key] = kvp.Value?.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not load existing form data for submission {SubmissionId}", SubmissionId);
                }
            }
            else
            {
                // Initialize form data with default values
                foreach (var field in Metadata.Fields.Where(f => f.IsVisible))
                {
                    if (!string.IsNullOrEmpty(field.DefaultValue))
                    {
                        FormData[field.FieldCode] = field.DefaultValue;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading form metadata");
            TempData["Error"] = "Lỗi khi tải form";
            return RedirectToPage("/Forms/Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrEmpty(Code))
        {
            return RedirectToPage("/Forms/Index");
        }

        try
        {
            // IMPORTANT:
            // On POST the form only sends FormData[...] by default, NOT Metadata.
            // So we must resolve metadata/version again (or use hidden FormVersionId).
            if (FormVersionId == Guid.Empty || Metadata?.Form == null)
            {
                Metadata = await _apiService.GetAsync<FormMetadata>($"/api/forms/code/{Code}/metadata");
                if (Metadata == null)
                {
                    TempData["Error"] = "Không tìm thấy form";
                    TempData["ErrorDetails"] = _apiService.LastError;
                    return RedirectToPage("/Forms/Index");
                }

                FormVersionId = Metadata.Version.Id;
            }

            var request = new
            {
                FormVersionId = FormVersionId,
                ObjectId = Guid.NewGuid().ToString(),
                ObjectType = Metadata!.Form.Code,
                Data = FormData
            };

            var result = await _apiService.PostAsync<object>("/api/formdata", request);
            if (result != null)
            {
                TempData["Success"] = "Lưu form thành công!";
                return RedirectToPage("/Forms/Index");
            }
            else
            {
                TempData["Error"] = "Lỗi khi lưu form";
                TempData["ErrorDetails"] = _apiService.LastError;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving form data");
            TempData["Error"] = "Lỗi khi lưu form: " + ex.Message;
            TempData["ErrorDetails"] = ex.ToString();
        }

        // Reload metadata
        var metadata = await _apiService.GetAsync<FormMetadata>($"/api/forms/code/{Code}/metadata");
        if (metadata != null)
        {
            Metadata = metadata;
            FormVersionId = metadata.Version.Id;
        }

        return Page();
    }
}
