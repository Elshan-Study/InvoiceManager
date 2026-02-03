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

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CustomerResponseDto>>>> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<CustomerResponseDto>>.SuccessResponse(customers, "List of customers retrieved successfully."));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CustomerResponseDto>>> GetById(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer is null)
            return NotFound(ApiResponse<CustomerResponseDto>.ErrorResponse($"Customer with ID {id} not found."));
        return Ok(ApiResponse<CustomerResponseDto>.SuccessResponse(customer, "Customer retrieved successfully."));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CustomerResponseDto>>> Create([FromBody] CreateCustomerDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<CustomerResponseDto>.ErrorResponse("Invalid model state."));

        var created = await _customerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<CustomerResponseDto>.SuccessResponse(created, "Customer created successfully."));
    }

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

    [HttpDelete("soft/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> SoftDelete(int id)
    {
        var success = await _customerService.DeleteSoftAsync(id);
        if (!success)
            return NotFound(ApiResponse<object>.ErrorResponse($"Customer with ID {id} not found."));
        return Ok(ApiResponse<object>.SuccessResponse(null, "Customer soft-deleted successfully."));
    }

    [HttpDelete("hard/{id}")]
    public async Task<ActionResult<ApiResponse<object>>> HardDelete(int id)
    {
        var success = await _customerService.DeleteHardAsync(id);
        if (!success)
            return BadRequest(ApiResponse<object>.ErrorResponse($"Cannot hard-delete customer with ID {id} (maybe has invoices or does not exist)."));
        return Ok(ApiResponse<object>.SuccessResponse(null, "Customer hard-deleted successfully."));
    }
}
