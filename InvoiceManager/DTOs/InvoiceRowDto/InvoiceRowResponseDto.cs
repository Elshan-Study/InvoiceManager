namespace InvoiceManager.DTOs.InvoiceRowDto;

public class InvoiceRowResponseDto
{
    public int Id { get; set; }
    public string Service { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Sum { get; set; }
}
