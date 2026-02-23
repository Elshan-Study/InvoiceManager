using FluentValidation;
using InvoiceManager.DTOs.AuthDto;

namespace InvoiceManager.Validators;

public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
{
    public UpdateProfileDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

        RuleFor(x => x.Address)
            .MaximumLength(250).WithMessage("Address cannot exceed 250 characters.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[0-9\s\-]{7,15}$")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Invalid phone number format.");
    }
}
