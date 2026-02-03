using InvoiceManager.Common;
using InvoiceManager.DTOs.InvoiceDto;
using InvoiceManager.Models;

namespace InvoiceManager.Services.Interfaces;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceResponseDto>> GetAllAsync();
    Task<InvoiceResponseDto?> GetByIdAsync(int id);
    Task<InvoiceResponseDto> CreateAsync(CreateInvoiceDto dto);
    Task<InvoiceResponseDto?> UpdateAsync(int id, UpdateInvoiceDto dto);
    Task<PagedResult<InvoiceResponseDto>> GetPagedAsync(
        int page = 1,
        int pageSize = 10,
        int? customerId = null,
        InvoiceStatus? status = null,
        DateTimeOffset? startFrom = null,
        DateTimeOffset? endTo = null,
        string? sortBy = "Id",
        bool ascending = true);
    Task<bool> ChangeStatusAsync(int id, InvoiceStatus status);
    Task<bool> DeleteHardAsync(int id);
    Task<bool> DeleteSoftAsync(int id);
}
