using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Suppliers.Commands.UpdateSupplier
{
    public class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
    {
        public UpdateSupplierCommandValidator()
        {
            RuleFor(x => x.SupplierId)
                .GreaterThan(0).WithMessage("Supplier ID must be greater than 0.");

            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(40).WithMessage("Company name must not exceed 40 characters.");

            RuleFor(x => x.ContactName)
                .MaximumLength(30).WithMessage("Contact name must not exceed 30 characters.")
                .When(x => x.ContactName != null);

            RuleFor(x => x.Phone)
                .MaximumLength(24).WithMessage("Phone must not exceed 24 characters.")
                .When(x => x.Phone != null);

            RuleFor(x => x.Country)
                .MaximumLength(15).WithMessage("Country must not exceed 15 characters.")
                .When(x => x.Country != null);
        }
    }

}