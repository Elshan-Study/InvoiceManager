namespace InvoiceManager.Models;

public class InvoiceRow
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string Service { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Sum { get; set; } //add Sum
}
