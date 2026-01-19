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

    [HttpGet("{id}")]
    public async Task<ActionResult<FormDataDto>> GetFormData(Guid id)
    {
        var formData = await _formDataService.GetFormDataAsync(id);
        if (formData == null) return NotFound();
        return Ok(formData);
    }

    [HttpGet("object/{objectId}/{objectType}")]
    public async Task<ActionResult<FormDataDto>> GetFormDataByObject(string objectId, string objectType)
    {
        var formData = await _formDataService.GetFormDataByObjectAsync(objectId, objectType);
        if (formData == null) return NotFound();
        return Ok(formData);
    }

    [HttpPost]
    public async Task<ActionResult<FormDataDto>> CreateFormData([FromBody] CreateFormDataRequest request)
    {
        try
        {
            var formData = await _formDataService.CreateFormDataAsync(request);
            return CreatedAtAction(nameof(GetFormData), new { id = formData.Id }, formData);
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

    [HttpPut("{id}")]
    public async Task<ActionResult<FormDataDto>> UpdateFormData(Guid id, [FromBody] CreateFormDataRequest request)
    {
        try
        {
            var formData = await _formDataService.UpdateFormDataAsync(id, request);
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
}

public class ValidateFormDataRequest
{
    public Guid FormVersionId { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
}
