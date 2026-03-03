using InvoiceManager.Models;

namespace InvoiceManager.DTOs.ReportDtos;

public class InvoiceStatusStatDto
{
    public InvoiceStatus Status { get; set; }
    public int InvoiceCount { get; set; }
}
