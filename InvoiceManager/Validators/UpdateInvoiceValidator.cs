using FluentValidation;
using InvoiceManager.DTOs.InvoiceDto;

namespace InvoiceManager.Validators;

public class UpdateInvoiceValidator : AbstractValidator<UpdateInvoiceDto>
{
    public UpdateInvoiceValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate).WithMessage("StartDate must be before EndDate");
    }
}
