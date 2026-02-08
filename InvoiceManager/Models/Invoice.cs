namespace InvoiceManager.Models;

public class Invoice
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTimeOffset StartDate { get; set; }
    public DateTimeOffset EndDate { get; set; }
    public virtual ICollection<InvoiceRow> Rows { get; set; } = new List<InvoiceRow>();
    public decimal TotalSum { get; set; } // decimal — заполняется в сервисе
    public string? Comment { get; set; } = string.Empty;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Created;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }  // НЕ nullable — обновляется при любом изменении
    public DateTimeOffset? DeletedAt { get; set; }
}
