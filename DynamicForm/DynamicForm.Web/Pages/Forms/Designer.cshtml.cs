using DynamicForm.Web.Models;
using DynamicForm.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace DynamicForm.Web.Pages.Forms;

public class DesignerModel : PageModel
{
    private readonly ApiService _apiService;
    private readonly ILogger<DesignerModel> _logger;

    public DesignerModel(ApiService apiService, ILogger<DesignerModel> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public List<FormInfo> Forms { get; set; } = new();
    public List<FormVersionInfo> Versions { get; set; } = new();
    public FormMetadata? Metadata { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Code { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? VersionId { get; set; }

    [BindProperty]
    public Guid VersionIdPost { get; set; }

    [BindProperty]
    public string? FieldsJson { get; set; }

    [BindProperty]
    public string? ChangeLog { get; set; }

    [BindProperty]
    public string? NewFormCode { get; set; }

    [BindProperty]
    public string? NewFormName { get; set; }

    [BindProperty]
    public string? NewFormDescription { get; set; }

    [BindProperty]
    public string? NewVersion { get; set; }

    [BindProperty]
    public Guid ActivateVersionId { get; set; }

    public async Task OnGetAsync(string? code, Guid? versionId)
    {
        Code = code;
        VersionId = versionId;

        try
        {
            Forms = await _apiService.GetAsync<List<FormInfo>>("/api/forms") ?? new();
            if (Forms.Count == 0 && !string.IsNullOrWhiteSpace(_apiService.LastError))
            {
                TempData["Error"] = "Không tải được danh sách form từ API. Bạn kiểm tra API có đang chạy không.";
                TempData["ErrorDetails"] = _apiService.LastError;
            }

            if (string.IsNullOrWhiteSpace(Code))
            {
                return;
            }

            var form = await _apiService.GetAsync<FormInfo>($"/api/forms/code/{Code}");
            if (form == null)
            {
                TempData["Error"] = "Không tìm thấy form";
                if (!string.IsNullOrWhiteSpace(_apiService.LastError))
                {
                    TempData["ErrorDetails"] = _apiService.LastError;
                }
                return;
            }

            Versions = await _apiService.GetAsync<List<FormVersionInfo>>($"/api/forms/{form.Id}/versions") ?? new();
            if (Versions.Count == 0 && !string.IsNullOrWhiteSpace(_apiService.LastError))
            {
                TempData["Error"] = "Không tải được danh sách version từ API.";
                TempData["ErrorDetails"] = _apiService.LastError;
            }

            // Resolve which version to load:
            // - If user passed versionId on query => use it
            // - Else, prefer CurrentVersionId (active version)
            // - Else, fall back to newest version if exists (common when versions exist but none activated yet)
            var resolvedVersionId =
                VersionId
                ?? form.CurrentVersionId
                ?? Versions.OrderByDescending(v => v.CreatedDate).FirstOrDefault()?.Id;

            if (resolvedVersionId == null)
            {
                TempData["Error"] = "Form chưa có version nào. Vui lòng tạo version trước.";
                return;
            }

            VersionId = resolvedVersionId;
            Metadata = await _apiService.GetAsync<FormMetadata>($"/api/forms/versions/{resolvedVersionId}/metadata");
            if (Metadata != null)
            {
                ChangeLog = Metadata.Version.ChangeLog;
            }
            else if (!string.IsNullOrWhiteSpace(_apiService.LastError))
            {
                TempData["Error"] = "Không tải được metadata của version.";
                TempData["ErrorDetails"] = _apiService.LastError;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading designer data");
            TempData["Error"] = "Lỗi khi tải dữ liệu thiết kế form";
            TempData["ErrorDetails"] = ex.ToString();
        }
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (VersionIdPost == Guid.Empty)
        {
            TempData["Error"] = "Thiếu VersionId";
            return RedirectToPage("/Forms/Designer", new { code = Code, versionId = VersionId });
        }

        try
        {
            var fields = string.IsNullOrWhiteSpace(FieldsJson)
                ? new List<FormFieldInfo>()
                : JsonSerializer.Deserialize<List<FormFieldInfo>>(FieldsJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<FormFieldInfo>();

            var request = new UpdateFormMetadataRequest
            {
                ChangeLog = ChangeLog,
                Fields = fields
            };

            var updated = await _apiService.PutAsync<FormMetadata>($"/api/forms/versions/{VersionIdPost}/metadata", request);
            if (updated == null)
            {
                TempData["Error"] = "Lỗi khi lưu metadata";
            }
            else
            {
                TempData["Success"] = "Lưu thiết kế thành công!";
            }

            // Try to redirect back using form code from returned metadata; fall back to query Code.
            var redirectCode = updated?.Form?.Code ?? Code;
            return RedirectToPage("/Forms/Designer", new { code = redirectCode, versionId = VersionIdPost });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving metadata");
            TempData["Error"] = "Lỗi khi lưu thiết kế: " + ex.Message;
            return RedirectToPage("/Forms/Designer", new { code = Code, versionId = VersionIdPost });
        }
    }

    public async Task<IActionResult> OnPostCreateFormAsync()
    {
        if (string.IsNullOrWhiteSpace(NewFormCode) || string.IsNullOrWhiteSpace(NewVersion))
        {
            TempData["Error"] = "Vui lòng nhập Code và Version";
            return RedirectToPage("/Forms/Designer");
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
                    return RedirectToPage("/Forms/Designer");
                }

                TempData["Success"] = $"Form đã tồn tại, đã tạo version {versionText}.";
                return RedirectToPage("/Forms/Designer", new { code = existingForm.Code, versionId = createdVersionForExisting.Id });
            }

            if (string.IsNullOrWhiteSpace(NewFormName))
            {
                TempData["Error"] = "Code chưa tồn tại, vui lòng nhập Name để tạo form mới";
                return RedirectToPage("/Forms/Designer");
            }

            // Create form
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
                return RedirectToPage("/Forms/Designer");
            }

            // Create version
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
                return RedirectToPage("/Forms/Designer", new { code = createdForm.Code });
            }

            TempData["Success"] = "Tạo form + version thành công!";
            return RedirectToPage("/Forms/Designer", new { code = createdForm.Code, versionId = createdVersion.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating form/version");
            TempData["Error"] = "Lỗi khi tạo form: " + ex.Message;
            return RedirectToPage("/Forms/Designer");
        }
    }

    public async Task<IActionResult> OnPostActivateVersionAsync()
    {
        if (ActivateVersionId == Guid.Empty)
        {
            TempData["Error"] = "Thiếu VersionId";
            return RedirectToPage("/Forms/Designer", new { code = Code, versionId = VersionId });
        }

        try
        {
            var result = await _apiService.PostAsync<object>($"/api/forms/versions/{ActivateVersionId}/activate", new { });
            if (result == null)
            {
                TempData["Error"] = "Không thể kích hoạt version";
            }
            else
            {
                TempData["Success"] = "Đã kích hoạt version!";
            }

            return RedirectToPage("/Forms/Designer", new { code = Code, versionId = ActivateVersionId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating version");
            TempData["Error"] = "Lỗi khi kích hoạt version: " + ex.Message;
            return RedirectToPage("/Forms/Designer", new { code = Code, versionId = ActivateVersionId });
        }
    }
}

