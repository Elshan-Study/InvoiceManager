using FluentValidation;
using InvoiceManager.DTOs.CustomerDto;

namespace InvoiceManager.Validators;

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerDto>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Customer name is required")
            .MinimumLength(2).WithMessage("Customer name must be at least 2 characters long");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+?[0-9\s\-]{7,15}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
            .WithMessage("Phone number is invalid");
    }
}