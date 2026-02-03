using InvoiceManager.DTOs.CustomerDto;

namespace InvoiceManager.Services.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerResponseDto>> GetAllAsync();
    Task<CustomerResponseDto?> GetByIdAsync(int id);
    Task<CustomerResponseDto> CreateAsync(CreateCustomerDto dto);
    Task<CustomerResponseDto?> UpdateAsync(int id, UpdateCustomerDto dto);

    Task<bool> DeleteHardAsync(int id);
    Task<bool> DeleteSoftAsync(int id);
}
