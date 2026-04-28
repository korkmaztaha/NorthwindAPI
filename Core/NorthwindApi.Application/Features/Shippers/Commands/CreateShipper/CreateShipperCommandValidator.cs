using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Shippers.Commands.CreateShipper
{
    public class CreateShipperCommandValidator : AbstractValidator<CreateShipperCommand>
    {
        public CreateShipperCommandValidator()
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(40).WithMessage("Company name must not exceed 40 characters.");

            RuleFor(x => x.Phone)
                .MaximumLength(24).WithMessage("Phone must not exceed 24 characters.")
                .When(x => x.Phone != null);
        }
    }
}