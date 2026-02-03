using InvoiceManager.Common;
using InvoiceManager.DTOs.CustomerDto;
using InvoiceManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    /// <summary>
    /// Retrieves all customers.
    /// </summary>
    /// <returns>List of all customers.</returns>
    /// <response code="200">Returns the list of customers successfully.</response>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CustomerResponseDto>>>> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<CustomerResponseDto>>.SuccessResponse(customers, "List of customers retrieved successfully."));
    }

    /// <summary>
    /// Retrieves a customer by its specific identifier.
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    /// <returns>The customer with the specified ID.</returns>
    /// <response code="200">Returns the customer if found.</response>
    /// <response code="404">If the customer is not found.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CustomerResponseDto>>> GetById(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer is null)
            return NotFound(ApiResponse<CustomerResponseDto>.ErrorResponse($"Customer with ID {id} not found."));
        return Ok(ApiResponse<CustomerResponseDto>.SuccessResponse(customer, "Customer retrieved successfully."));
    }

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="dto">Customer data to create.</param>
    /// <returns>The created customer.</returns>
    /// <response code="201">Returns the newly created customer.</response>
    /// <response code="400">If the model is invalid.</response>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CustomerResponseDto>>> Create([FromBody] CreateCustomerDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<CustomerResponseDto>.ErrorResponse("Invalid model state."));

        var created = await _customerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<CustomerResponseDto>.SuccessResponse(created, "Customer created successfully."));
    }

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    /// <param name="dto">Updated customer data.</param>
    /// <returns>The updated customer.</returns>
    /// <response code="200">Returns the updated customer.</response>
    /// <response code="400">If the model is invalid.</response>
    /// <response code="404">If the customer is not found.</response>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CustomerResponseDto>>> Update(int id, [FromBody] UpdateCustomerDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<CustomerResponseDto>.ErrorResponse("Invalid model state."));

        var updated = await _customerService.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound(ApiResponse<CustomerResponseDto>.ErrorResponse($"Customer with ID {id} not found."));

        return Ok(ApiResponse<CustomerResponseDto>.SuccessResponse(updated, "Customer updated successfully."));
    }

    /// <summary>
    /// Soft-deletes a customer (marks as deleted).
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    /// <response code="200">Customer soft-deleted successfully.</response>
    /// <response code="404">If the customer is not found.</response>
    [HttpDelete("soft/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> SoftDelete(int id)
    {
        var success = await _customerService.DeleteSoftAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.ErrorResponse($"Customer with ID {id} not found."));
        return Ok(ApiResponse<object>.SuccessResponse(null, "Customer soft-deleted successfully."));
    }

    /// <summary>
    /// Hard-deletes a customer (removes from database).
    /// </summary>
    /// <param name="id">Customer identifier.</param>
    /// <response code="200">Customer hard-deleted successfully.</response>
    /// <response code="400">If the customer cannot be deleted (maybe has invoices).</response>
    [HttpDelete("hard/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> HardDelete(int id)
    {
        var success = await _customerService.DeleteHardAsync(id);
        if (!success)
            return BadRequest(ApiResponse<object>.ErrorResponse($"Cannot hard-delete customer with ID {id} (maybe has invoices or does not exist)."));
        return Ok(ApiResponse<object>.SuccessResponse(null, "Customer hard-deleted successfully."));
    }

    /// <summary>
    /// Retrieves a paged list of customers with optional filtering and sorting.
    /// </summary>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="pageSize">Number of items per page (default 10).</param>
    /// <param name="name">Optional filter by customer name.</param>
    /// <param name="sortBy">Property to sort by (default Id).</param>
    /// <param name="ascending">Sort direction (true = ascending, false = descending).</param>
    /// <returns>Paged result of customers.</returns>
    /// <response code="200">Returns paged customers successfully.</response>
    [HttpGet("paged")]
    public async Task<ActionResult<ApiResponse<PagedResult<CustomerResponseDto>>>> GetPaged(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? name = null,
    [FromQuery] string? sortBy = "Id",
    [FromQuery] bool ascending = true)
    {
        var result = await _customerService.GetPagedAsync(page, pageSize, name, sortBy, ascending);
        return Ok(ApiResponse<PagedResult<CustomerResponseDto>>.SuccessResponse(result, "Paged customers retrieved successfully"));
    }

}
