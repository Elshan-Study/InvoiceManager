namespace InvoiceManager.DTOs.InvoiceRowDto;

public class CreateInvoiceRowDto
{
    public string Service { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
}


