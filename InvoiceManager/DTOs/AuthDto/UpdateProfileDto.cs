namespace InvoiceManager.DTOs.AuthDto;

public class UpdateProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
}
