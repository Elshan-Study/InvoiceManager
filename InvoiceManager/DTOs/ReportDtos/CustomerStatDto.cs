namespace InvoiceManager.DTOs.ReportDtos;

public class CustomerStatDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int InvoiceCount { get; set; }
    public decimal TotalSum { get; set; }
}
