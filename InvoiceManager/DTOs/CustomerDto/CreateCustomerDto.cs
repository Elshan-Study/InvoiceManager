using System.ComponentModel;

namespace InvoiceManager.DTOs.CustomerDto;

public class CreateCustomerDto
{
    [DefaultValue("John Doe")]
    public string Name { get; set; } = string.Empty;

    [DefaultValue("123 Main St, City")]
    public string? Address { get; set; }

    [DefaultValue("john.doe@example.com")]
    public string Email { get; set; } = string.Empty;

    [DefaultValue("+1-555-123-4567")]
    public string? PhoneNumber { get; set; }
}
