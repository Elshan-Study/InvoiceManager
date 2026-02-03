using FluentValidation;
using InvoiceManager.DTOs.InvoiceDto;

namespace InvoiceManager.Validators;

public class CreateInvoiceValidator : AbstractValidator<CreateInvoiceDto>
{
    public CreateInvoiceValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate).WithMessage("StartDate must be before EndDate");

        RuleFor(x => x.Rows)
            .NotEmpty().WithMessage("Invoice must have at least one row");

        RuleForEach(x => x.Rows).SetValidator(new CreateInvoiceRowValidator());
    }
}
