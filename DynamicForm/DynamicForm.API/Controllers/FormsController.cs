using DynamicForm.API.DTOs;
using DynamicForm.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace DynamicForm.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormsController : ControllerBase
{
    private readonly IFormService _formService;

    public FormsController(IFormService formService)
    {
        _formService = formService;
    }

    [HttpGet]
    public async Task<ActionResult<List<FormDto>>> GetAllForms()
    {
        var forms = await _formService.GetAllFormsAsync();
        return Ok(forms);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FormDto>> GetFormById(Guid id)
    {
        var form = await _formService.GetFormByIdAsync(id);
        if (form == null) return NotFound();
        return Ok(form);
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<FormDto>> GetFormByCode(string code)
    {
        var form = await _formService.GetFormByCodeAsync(code);
        if (form == null) return NotFound();
        return Ok(form);
    }

    [HttpGet("code/{code}/metadata")]
    public async Task<ActionResult<FormMetadataDto>> GetFormMetadata(string code)
    {
        var metadata = await _formService.GetFormMetadataAsync(code);
        if (metadata == null) return NotFound();
        return Ok(metadata);
    }

    [HttpGet("{formId}/versions")]
    public async Task<ActionResult<List<FormVersionDto>>> GetVersionsByFormId(Guid formId)
    {
        var versions = await _formService.GetVersionsByFormIdAsync(formId);
        return Ok(versions);
    }

    [HttpGet("versions/{versionId}/metadata")]
    public async Task<ActionResult<FormMetadataDto>> GetFormMetadataByVersion(Guid versionId)
    {
        var metadata = await _formService.GetFormMetadataByVersionIdAsync(versionId);
        if (metadata == null) return NotFound();
        return Ok(metadata);
    }

    [HttpPut("versions/{versionId}/metadata")]
    public async Task<ActionResult<FormMetadataDto>> UpdateFormMetadata(Guid versionId, [FromBody] UpdateFormMetadataRequest request)
    {
        try
        {
            var metadata = await _formService.UpdateFormMetadataByVersionIdAsync(versionId, request);
            if (metadata == null) return NotFound();
            return Ok(metadata);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<FormDto>> CreateForm([FromBody] FormDto formDto)
    {
        try
        {
            var form = await _formService.CreateFormAsync(formDto);
            return CreatedAtAction(nameof(GetFormById), new { id = form.Id }, form);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (DbUpdateException ex)
        {
            var detail = ex.InnerException?.Message ?? ex.Message;
            return BadRequest(new { error = ex.Message, detail });
        }
        catch (Exception ex)
        {
            var detail = ex.InnerException?.Message;
            return BadRequest(new { error = ex.Message, detail });
        }
    }

    [HttpPost("{formId}/versions")]
    public async Task<ActionResult<FormVersionDto>> CreateVersion(Guid formId, [FromBody] FormVersionDto versionDto)
    {
        try
        {
            var version = await _formService.CreateVersionAsync(formId, versionDto);
            return CreatedAtAction(nameof(GetFormMetadataByVersion), new { versionId = version.Id }, version);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (DbUpdateException ex)
        {
            var detail = ex.InnerException?.Message ?? ex.Message;
            return BadRequest(new { error = ex.Message, detail });
        }
        catch (Exception ex)
        {
            var detail = ex.InnerException?.Message;
            return BadRequest(new { error = ex.Message, detail });
        }
    }

    [HttpPost("versions/{versionId}/activate")]
    public async Task<ActionResult> ActivateVersion(Guid versionId)
    {
        var result = await _formService.ActivateVersionAsync(versionId);
        if (!result) return NotFound();
        return Ok(new { message = "Version activated successfully" });
    }

    [HttpPost("{formId}/deactivate")]
    public async Task<ActionResult> DeactivateForm(Guid formId)
    {
        var result = await _formService.DeactivateFormAsync(formId);
        if (!result) return NotFound();
        return Ok(new { message = "Form deactivated successfully" });
    }
}
