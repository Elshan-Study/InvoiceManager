using InvoiceManager.Common;
using InvoiceManager.DTOs.InvoiceDto;
using InvoiceManager.DTOs.ReportDtos;
using InvoiceManager.Models;
using InvoiceManager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InvoiceManager.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;
    private readonly IInvoiceExportService _exportService;
    private readonly IInvoiceReportService _reportService;
    private int GetCurrentUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    public InvoicesController(IInvoiceService invoiceService,
                          IInvoiceExportService exportService,
                          IInvoiceReportService reportService)
    {
        _invoiceService = invoiceService;
        _exportService = exportService;
        _reportService = reportService;
    }

    /// <summary>
    /// Retrieves all invoices.
    /// </summary>
    /// <returns>List of all invoices.</returns>
    /// <response code="200">Returns the list of invoices successfully.</response>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<InvoiceResponseDto>>>> GetAll()
    {
        var invoices = await _invoiceService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<InvoiceResponseDto>>.SuccessResponse(invoices, "List of invoices retrieved successfully."));
    }

    /// <summary>
    /// Retrieves an invoice by its specific identifier.
    /// </summary>
    /// <param name="id">Invoice identifier.</param>
    /// <returns>The invoice with the specified ID.</returns>
    /// <response code="200">Returns the invoice if found.</response>
    /// <response code="404">If the invoice is not found.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<InvoiceResponseDto>>> GetById(int id)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);
        if (invoice is null)
            return NotFound(ApiResponse<InvoiceResponseDto>.ErrorResponse($"Invoice with ID {id} not found."));
        return Ok(ApiResponse<InvoiceResponseDto>.SuccessResponse(invoice, "Invoice retrieved successfully."));
    }

    /// <summary>
    /// Creates a new invoice.
    /// </summary>
    /// <param name="dto">Invoice data to create.</param>
    /// <returns>The created invoice.</returns>
    /// <response code="201">Returns the newly created invoice.</response>
    /// <response code="400">If the model is invalid.</response>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<InvoiceResponseDto>>> Create([FromBody] CreateInvoiceDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<InvoiceResponseDto>.ErrorResponse("Invalid model state."));

        var created = await _invoiceService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<InvoiceResponseDto>.SuccessResponse(created, "Invoice created successfully."));
    }


    /// <summary>
    /// Updates an existing invoice.
    /// </summary>
    /// <param name="id">Invoice identifier.</param>
    /// <param name="dto">Updated invoice data.</param>
    /// <returns>The updated invoice.</returns>
    /// <response code="200">Returns the updated invoice.</response>
    /// <response code="400">If the model is invalid or invoice cannot be updated.</response>
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

    /// <summary>
    /// Changes the status of an invoice.
    /// </summary>
    /// <param name="id">Invoice identifier.</param>
    /// <param name="dto">New status.</param>
    /// <response code="200">Invoice status changed successfully.</response>
    /// <response code="400">If the status cannot be changed.</response>
    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<object>>> ChangeStatus(int id, [FromBody] ChangeInvoiceStatusDto dto)
    {
        var success = await _invoiceService.ChangeStatusAsync(id, dto.Status);
        if (!success)
            return BadRequest(ApiResponse<object>.ErrorResponse($"Cannot change status for invoice with ID {id}."));

        return Ok(ApiResponse<object>.SuccessResponse(null, "Invoice status changed successfully."));
    }


    /// <summary>
    /// Soft-deletes an invoice (marks as deleted).
    /// </summary>
    /// <param name="id">Invoice identifier.</param>
    /// <response code="200">Invoice soft-deleted successfully.</response>
    /// <response code="404">If the invoice is not found.</response>
    [HttpDelete("soft/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> SoftDelete(int id)
    {
        var success = await _invoiceService.DeleteSoftAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.ErrorResponse($"Invoice with ID {id} not found."));
        return Ok(ApiResponse<object>.SuccessResponse(null, "Invoice soft-deleted successfully."));
    }

    /// <summary>
    /// Hard-deletes an invoice (removes from database).
    /// </summary>
    /// <param name="id">Invoice identifier.</param>
    /// <response code="200">Invoice hard-deleted successfully.</response>
    /// <response code="400">If the invoice cannot be deleted (maybe already sent or does not exist).</response>
    [HttpDelete("hard/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> HardDelete(int id)
    {
        var success = await _invoiceService.DeleteHardAsync(id);
        if (!success)
            return BadRequest(ApiResponse<object>.ErrorResponse($"Cannot hard-delete invoice with ID {id} (maybe already sent or does not exist)."));
        return Ok(ApiResponse<object>.SuccessResponse(null, "Invoice hard-deleted successfully."));
    }

    /// <summary>
    /// Retrieves a paged list of invoices with optional filtering and sorting.
    /// </summary>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="pageSize">Number of items per page (default 10).</param>
    /// <param name="customerId">Optional filter by customer ID.</param>
    /// <param name="status">Optional filter by invoice status.</param>
    /// <param name="startFrom">Optional filter: start date from.</param>
    /// <param name="endTo">Optional filter: end date to.</param>
    /// <param name="sortBy">Property to sort by (default Id).</param>
    /// <param name="ascending">Sort direction (true = ascending, false = descending).</param>
    /// <returns>Paged result of invoices.</returns>
    /// <response code="200">Returns paged invoices successfully.</response>
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

    /// <summary>
    /// Downloads invoice as PDF or DOCX.
    /// </summary>
    [HttpGet("{id}/download")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Download(int id, [FromQuery] InvoiceExportFormat format)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);
        if (invoice is null)
            return NotFound();

        var fileBytes = await _exportService.ExportAsync(invoice, format);

        var contentType = format switch
        {
            InvoiceExportFormat.Pdf =>
                "application/pdf",

            InvoiceExportFormat.Docx =>
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",

            _ => "application/octet-stream"
        };

        return new FileContentResult(fileBytes, contentType)
        {
            FileDownloadName = $"invoice_{id}.{format.ToString().ToLower()}"
        };
    }


    /// <summary>
    /// Returns customer statistics: invoice count and total amount for the specified period.
    /// </summary>
    /// <param name="startFrom">Period start date (filters by invoice StartDate).</param>
    /// <param name="endTo">Period end date (filters by invoice EndDate).</param>
    /// <response code="200">Returns customer statistics successfully.</response>
    [HttpGet("customers")]
    public async Task<ActionResult<ApiResponse<IEnumerable<CustomerStatDto>>>> GetCustomerStats(
        [FromQuery] DateTimeOffset? startFrom = null,
        [FromQuery] DateTimeOffset? endTo = null)
    {
        var result = await _reportService.GetCustomerStatsAsync(startFrom, endTo);
        return Ok(ApiResponse<IEnumerable<CustomerStatDto>>
            .SuccessResponse(result, "Customer statistics retrieved successfully."));
    }

    /// <summary>
    /// Returns work statistics: invoice count and total amount per service for the specified period.
    /// </summary>
    /// <param name="startFrom">Period start date (filters by invoice StartDate).</param>
    /// <param name="endTo">Period end date (filters by invoice EndDate).</param>
    /// <response code="200">Returns work statistics successfully.</response>
    [HttpGet("works")]
    public async Task<ActionResult<ApiResponse<IEnumerable<WorkStatDto>>>> GetWorkStats(
        [FromQuery] DateTimeOffset? startFrom = null,
        [FromQuery] DateTimeOffset? endTo = null)
    {
        var result = await _reportService.GetWorkStatsAsync(startFrom, endTo);
        return Ok(ApiResponse<IEnumerable<WorkStatDto>>
            .SuccessResponse(result, "Work statistics retrieved successfully."));
    }

    /// <summary>
    /// Returns invoice statistics: invoice count grouped by status for the specified period.
    /// </summary>
    /// <param name="startFrom">Period start date (filters by invoice StartDate).</param>
    /// <param name="endTo">Period end date (filters by invoice EndDate).</param>
    /// <response code="200">Returns invoice status statistics successfully.</response>
    [HttpGet("statuses")]
    public async Task<ActionResult<ApiResponse<IEnumerable<InvoiceStatusStatDto>>>> GetStatusStats(
        [FromQuery] DateTimeOffset? startFrom = null,
        [FromQuery] DateTimeOffset? endTo = null)
    {
        var result = await _reportService.GetStatusStatsAsync(startFrom, endTo);
        return Ok(ApiResponse<IEnumerable<InvoiceStatusStatDto>>
            .SuccessResponse(result, "Invoice status statistics retrieved successfully."));
    }
}
