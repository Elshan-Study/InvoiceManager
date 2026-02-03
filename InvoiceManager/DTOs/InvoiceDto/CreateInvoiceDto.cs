using InvoiceManager.DTOs.InvoiceRowDto;

namespace InvoiceManager.DTOs.InvoiceDto;

public class CreateInvoiceDto
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string? Comment { get; set; }
    public List<CreateInvoiceRowDto> Rows { get; set; } = new();
}
