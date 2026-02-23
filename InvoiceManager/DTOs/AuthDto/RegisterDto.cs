using System.ComponentModel;

namespace InvoiceManager.DTOs.AuthDto;

public class RegisterDto
{
    [DefaultValue("John Doe")]
    public string Name { get; set; } = string.Empty;

    [DefaultValue("john.doe@example.com")]
    public string Email { get; set; } = string.Empty;

    [DefaultValue("P@ssw0rd123")]
    public string Password { get; set; } = string.Empty;
}
