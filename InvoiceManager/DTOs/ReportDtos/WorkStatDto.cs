namespace InvoiceManager.DTOs.ReportDtos;

public class WorkStatDto
{
    public string WorkName { get; set; } = string.Empty;
    public int InvoiceCount { get; set; }
    public decimal TotalSum { get; set; }
}
