using DynamicForm.API.DTOs;
using DynamicForm.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace DynamicForm.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormDataController : ControllerBase
{
    private readonly IFormDataService _formDataService;

    public FormDataController(IFormDataService formDataService)
    {
        _formDataService = formDataService;
    }

    [HttpGet("{submissionId}")]
    public async Task<ActionResult<FormDataDto>> GetFormData(int submissionId)
    {
        var formData = await _formDataService.GetFormDataAsync(submissionId);
        if (formData == null) return NotFound();
        return Ok(formData);
    }

    [HttpGet("object/{objectId}/{objectType}/{formVersionPublicId}")]
    public async Task<ActionResult<FormDataDto>> GetFormDataByObject(string objectId, string objectType, Guid formVersionPublicId)
    {
        var formData = await _formDataService.GetFormDataByObjectAsync(objectId, objectType, formVersionPublicId);
        if (formData == null) return NotFound();
        return Ok(formData);
    }

    [HttpPost]
    public async Task<ActionResult<FormDataDto>> CreateFormData([FromBody] CreateFormDataRequest request)
    {
        try
        {
            var formData = await _formDataService.CreateFormDataAsync(request);
            // Note: FormDataDto.Id là Guid (PublicId), nhưng GetFormDataAsync nhận int (SubmissionId)
            // Cần lấy SubmissionId từ response hoặc tạo cách khác
            // Tạm thời return Created với formData
            return CreatedAtAction(nameof(GetFormData), new { submissionId = 0 }, formData);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("{submissionId}")]
    public async Task<ActionResult<FormDataDto>> UpdateFormData(int submissionId, [FromBody] CreateFormDataRequest request)
    {
        try
        {
            var formData = await _formDataService.UpdateFormDataAsync(submissionId, request);
            return Ok(formData);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("validate")]
    public async Task<ActionResult<ValidationResultDto>> ValidateFormData([FromBody] ValidateFormDataRequest request)
    {
        var result = await _formDataService.ValidateFormDataAsync(request.FormVersionId, request.Data);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<ActionResult<List<FormDataListItemDto>>> GetFormDataList(
        [FromQuery] Guid? formVersionId = null,
        [FromQuery] string? objectType = null,
        [FromQuery] string? objectId = null)
    {
        var list = await _formDataService.GetFormDataListAsync(formVersionId, objectType, objectId);
        return Ok(list);
    }
}

public class ValidateFormDataRequest
{
    public Guid FormVersionId { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
}
