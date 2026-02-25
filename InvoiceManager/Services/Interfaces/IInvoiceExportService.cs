using InvoiceManager.DTOs.InvoiceDto;
using InvoiceManager.Models;

namespace InvoiceManager.Services.Interfaces;

public interface IInvoiceExportService
{
    Task<byte[]> ExportAsync(InvoiceResponseDto invoice, InvoiceExportFormat format);
}
