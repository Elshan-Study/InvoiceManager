using FluentValidation;
using InvoiceManager.DTOs.InvoiceDto;

namespace InvoiceManager.Validators;

public class ChangeInvoiceStatusValidator : AbstractValidator<ChangeInvoiceStatusDto>
{
    public ChangeInvoiceStatusValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid invoice status");
    }
}
