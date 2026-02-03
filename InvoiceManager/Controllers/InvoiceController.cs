using InvoiceManager.Common;
using InvoiceManager.DTOs.InvoiceDto;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoicesController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<InvoiceResponseDto>>>> GetAll()
    {
        var invoices = await _invoiceService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<InvoiceResponseDto>>.SuccessResponse(invoices, "List of invoices retrieved successfully."));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<InvoiceResponseDto>>> GetById(int id)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);
        if (invoice is null)
            return NotFound(ApiResponse<InvoiceResponseDto>.ErrorResponse($"Invoice with ID {id} not found."));
        return Ok(ApiResponse<InvoiceResponseDto>.SuccessResponse(invoice, "Invoice retrieved successfully."));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<InvoiceResponseDto>>> Create([FromBody] CreateInvoiceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<InvoiceResponseDto>.ErrorResponse("Invalid model state."));

        var created = await _invoiceService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<InvoiceResponseDto>.SuccessResponse(created, "Invoice created successfully."));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<InvoiceResponseDto>>> Update(int id, [FromBody] UpdateInvoiceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<InvoiceResponseDto>.ErrorResponse("Invalid model state."));

        var updated = await _invoiceService.UpdateAsync(id, dto);
        if (updated is null)
            return BadRequest(ApiResponse<InvoiceResponseDto>.ErrorResponse($"Invoice cannot be updated (maybe not found or already sent)."));

        return Ok(ApiResponse<InvoiceResponseDto>.SuccessResponse(updated, "Invoice updated successfully."));
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<object>>> ChangeStatus(int id, [FromBody] ChangeInvoiceStatusDto dto)
    {
        var success = await _invoiceService.ChangeStatusAsync(id, dto.Status);
        if (!success)
            return BadRequest(ApiResponse<object>.ErrorResponse($"Cannot change status for invoice with ID {id}."));

        return Ok(ApiResponse<object>.SuccessResponse(null, "Invoice status changed successfully."));
    }

    [HttpDelete("soft/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> SoftDelete(int id)
    {
        var success = await _invoiceService.DeleteSoftAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.ErrorResponse($"Invoice with ID {id} not found."));
        return Ok(ApiResponse<object>.SuccessResponse(null, "Invoice soft-deleted successfully."));
    }

    [HttpDelete("hard/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> HardDelete(int id)
    {
        var success = await _invoiceService.DeleteHardAsync(id);
        if (!success)
            return BadRequest(ApiResponse<object>.ErrorResponse($"Cannot hard-delete invoice with ID {id} (maybe already sent or does not exist)."));
        return Ok(ApiResponse<object>.SuccessResponse(null, "Invoice hard-deleted successfully."));
    }

    [HttpGet("paged")]
    public async Task<ActionResult<ApiResponse<PagedResult<InvoiceResponseDto>>>> GetPaged(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] int? customerId = null,
    [FromQuery] InvoiceStatus? status = null,
    [FromQuery] DateTimeOffset? startFrom = null,
    [FromQuery] DateTimeOffset? endTo = null,
    [FromQuery] string? sortBy = "Id",
    [FromQuery] bool ascending = true)
    {
        var result = await _invoiceService.GetPagedAsync(page, pageSize, customerId, status, startFrom, endTo, sortBy, ascending);
        return Ok(ApiResponse<PagedResult<InvoiceResponseDto>>.SuccessResponse(result, "Paged invoices retrieved successfully"));
    }

}
