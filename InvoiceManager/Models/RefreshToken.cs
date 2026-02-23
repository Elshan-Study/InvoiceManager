namespace InvoiceManager.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset Expires { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }

    public virtual User? User { get; set; }
    public string? CreatedByIp { get; set; }
}
