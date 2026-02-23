namespace InvoiceManager.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
