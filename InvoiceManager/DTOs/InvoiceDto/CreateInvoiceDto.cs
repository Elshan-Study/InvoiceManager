using InvoiceManager.DTOs.InvoiceRowDto;
using System.ComponentModel;

namespace InvoiceManager.DTOs.InvoiceDto;

public class CreateInvoiceDto
{
    [DefaultValue("2026-02-03T09:00:00Z")]
    public DateTimeOffset StartDate { get; set; }

    [DefaultValue("2026-02-03T17:00:00Z")]
    public DateTimeOffset EndDate { get; set; }

    [DefaultValue("Monthly service invoice")]
    public string? Comment { get; set; }

    [DefaultValue(null)]
    public List<CreateInvoiceRowDto> Rows { get; set; } = new();
}
