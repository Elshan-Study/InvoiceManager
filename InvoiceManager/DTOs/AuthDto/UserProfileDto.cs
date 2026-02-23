namespace InvoiceManager.DTOs.AuthDto;

public class UserProfileDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
