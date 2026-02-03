using InvoiceManager.DTOs.InvoiceDto;
using InvoiceManager.Models;

namespace InvoiceManager.Services.Interfaces;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceResponseDto>> GetAllAsync();
    Task<InvoiceResponseDto?> GetByIdAsync(int id);
    Task<InvoiceResponseDto> CreateAsync(CreateInvoiceDto dto);
    Task<InvoiceResponseDto?> UpdateAsync(int id, UpdateInvoiceDto dto);
    Task<bool> ChangeStatusAsync(int id, InvoiceStatus status);
    Task<bool> DeleteHardAsync(int id);
    Task<bool> DeleteSoftAsync(int id);
}
