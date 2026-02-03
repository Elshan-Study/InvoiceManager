namespace InvoiceManager.DTOs.InvoiceDto;

public class UpdateInvoiceDto
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string? Comment { get; set; }
}
