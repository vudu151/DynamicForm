using DynamicForm.Web.Models;
using DynamicForm.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace DynamicForm.Web.Pages.Forms;

public class IndexModel : PageModel
{
    private readonly ApiService _apiService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ApiService apiService, ILogger<IndexModel> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public List<FormInfo> Forms { get; set; } = new();

    [BindProperty]
    public string? NewFormCode { get; set; }

    [BindProperty]
    public string? NewFormName { get; set; }

    [BindProperty]
    public string? NewFormDescription { get; set; }

    [BindProperty]
    public string? NewVersion { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            var forms = await _apiService.GetAsync<List<FormInfo>>("/api/forms");
            Forms = forms ?? new List<FormInfo>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading forms");
            Forms = new List<FormInfo>();
        }
    }

    public async Task<IActionResult> OnPostCreateFormAsync()
    {
        if (string.IsNullOrWhiteSpace(NewFormCode) || string.IsNullOrWhiteSpace(NewVersion))
        {
            TempData["Error"] = "Vui lòng nhập Code và Version";
            return RedirectToPage("/Forms/Index");
        }

        try
        {
            var code = NewFormCode.Trim();
            var versionText = NewVersion.Trim();

            // If form exists -> create a new version instead of creating a new form
            var existingForm = await _apiService.GetAsync<FormInfo>($"/api/forms/code/{code}");
            if (existingForm != null)
            {
                var createdVersionForExisting = await _apiService.PostAsync<FormVersionInfo>(
                    $"/api/forms/{existingForm.Id}/versions",
                    new
                    {
                        FormId = existingForm.Id,
                        Version = versionText,
                        IsActive = false,
                        CreatedBy = "admin",
                        ChangeLog = $"Create version {versionText}"
                    });

                if (createdVersionForExisting == null)
                {
                    TempData["Error"] = "Form đã tồn tại nhưng không thể tạo version (có thể version bị trùng).";
                    TempData["ErrorDetails"] = _apiService.LastError;
                    return RedirectToPage("/Forms/Index");
                }

                TempData["Success"] = $"Form đã tồn tại, đã tạo version {versionText}.";
                return RedirectToPage("/Forms/Designer", new { code = existingForm.Code, versionId = createdVersionForExisting.Id });
            }

            if (string.IsNullOrWhiteSpace(NewFormName))
            {
                TempData["Error"] = "Code chưa tồn tại, vui lòng nhập Name để tạo form mới";
                return RedirectToPage("/Forms/Index");
            }

            var formRequest = new
            {
                Code = code,
                Name = NewFormName.Trim(),
                Description = string.IsNullOrWhiteSpace(NewFormDescription) ? null : NewFormDescription.Trim(),
                Status = 0,
                CreatedBy = "admin"
            };

            var createdForm = await _apiService.PostAsync<FormInfo>("/api/forms", formRequest);
            if (createdForm == null)
            {
                TempData["Error"] = "Không thể tạo form";
                TempData["ErrorDetails"] = _apiService.LastError;
                return RedirectToPage("/Forms/Index");
            }

            var versionRequest = new
            {
                FormId = createdForm.Id,
                Version = versionText,
                IsActive = false,
                CreatedBy = "admin",
                ChangeLog = "Initial version"
            };

            var createdVersion = await _apiService.PostAsync<FormVersionInfo>($"/api/forms/{createdForm.Id}/versions", versionRequest);
            if (createdVersion == null)
            {
                TempData["Error"] = "Tạo form thành công nhưng không thể tạo version";
                if (!string.IsNullOrWhiteSpace(_apiService.LastError))
                {
                    TempData["Error"] += "\n" + _apiService.LastError;
                }
                return RedirectToPage("/Forms/Index");
            }

            TempData["Success"] = "Tạo form + version thành công!";
            return RedirectToPage("/Forms/Designer", new { code = createdForm.Code, versionId = createdVersion.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating form/version");
            TempData["Error"] = "Lỗi khi tạo form: " + ex.Message;
            return RedirectToPage("/Forms/Index");
        }
    }

    public async Task<IActionResult> OnPostDeactivateAsync(Guid formId)
    {
        try
        {
            var result = await _apiService.PostAsync<object>($"/api/forms/{formId}/deactivate", new { });
            TempData[result == null ? "Error" : "Success"] = result == null ? "Không thể ngưng kích hoạt form" : "Đã ngưng kích hoạt form";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating form");
            TempData["Error"] = "Lỗi khi ngưng kích hoạt form: " + ex.Message;
        }

        return RedirectToPage("/Forms/Index");
    }
}
