using InvoiceManager.Common;
using InvoiceManager.DTOs.CustomerDto;

namespace InvoiceManager.Services.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerResponseDto>> GetAllAsync();
    Task<CustomerResponseDto?> GetByIdAsync(int id);
    Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto);
    Task<CustomerResponseDto?> UpdateAsync(int id, UpdateCustomerDto dto);
    Task<PagedResult<CustomerResponseDto>> GetPagedAsync(
        int page = 1,
        int pageSize = 10,
        string? nameFilter = null,
        string? sortBy = "Id",
        bool ascending = true);

    Task<bool> DeleteHardAsync(int id);
    Task<bool> DeleteSoftAsync(int id);
}
