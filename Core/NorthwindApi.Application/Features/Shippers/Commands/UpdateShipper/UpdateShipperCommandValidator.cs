using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Shippers.Commands.UpdateShipper
{
    public class UpdateShipperCommandValidator : AbstractValidator<UpdateShipperCommand>
    {
        public UpdateShipperCommandValidator()
        {
            RuleFor(x => x.ShipperId)
                .GreaterThan(0).WithMessage("Shipper ID must be greater than 0.");

            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(40).WithMessage("Company name must not exceed 40 characters.");

            RuleFor(x => x.Phone)
                .MaximumLength(24).WithMessage("Phone must not exceed 24 characters.")
                .When(x => x.Phone != null);
        }
    }
}
