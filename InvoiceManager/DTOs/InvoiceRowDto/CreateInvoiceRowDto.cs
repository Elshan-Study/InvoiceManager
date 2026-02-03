using System.ComponentModel;

namespace InvoiceManager.DTOs.InvoiceRowDto;

public class CreateInvoiceRowDto
{
    [DefaultValue("Consulting Service")]
    public string Service { get; set; } = string.Empty;

    [DefaultValue(1)]
    public decimal Quantity { get; set; }

    [DefaultValue(100)]
    public decimal Rate { get; set; }
}


