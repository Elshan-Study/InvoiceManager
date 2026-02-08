using InvoiceManager.DTOs.InvoiceRowDto;

namespace InvoiceManager.DTOs.InvoiceDto;

public class UpdateInvoiceDto
{
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public string? Comment { get; set; }
    public List<CreateInvoiceRowDto>? Rows { get; set; } = null; // если null — не менять состав, если задан — заменить/синхронизировать

}
