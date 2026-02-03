using FluentValidation;
using InvoiceManager.DTOs.InvoiceRowDto;

namespace InvoiceManager.Validators;

public class CreateInvoiceRowValidator : AbstractValidator<CreateInvoiceRowDto>
{
    public CreateInvoiceRowValidator()
    {
        RuleFor(x => x.Service)
            .NotEmpty().WithMessage("Service name is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Rate)
            .GreaterThan(0).WithMessage("Rate must be greater than 0");
    }
}
