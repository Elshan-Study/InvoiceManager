using InvoiceManager.Models;

namespace InvoiceManager.DTOs.InvoiceDto;

public class ChangeInvoiceStatusDto
{
    public InvoiceStatus Status { get; set; }
}
