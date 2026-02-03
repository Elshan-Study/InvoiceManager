using InvoiceManager.DTOs.InvoiceRowDto;
using InvoiceManager.Models;

namespace InvoiceManager.DTOs.InvoiceDto;

public class InvoiceResponseDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public List<InvoiceRowResponseDto> Rows { get; set; } = new();
    public decimal TotalSum { get; set; }
    public string? Comment { get; set; }
    public InvoiceStatus Status { get; set; }
}

