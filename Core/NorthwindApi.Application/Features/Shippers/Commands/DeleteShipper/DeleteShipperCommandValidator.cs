using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Shippers.Commands.DeleteShipper
{
    public class DeleteShipperCommandValidator : AbstractValidator<DeleteShipperCommand>
    {
        public DeleteShipperCommandValidator()
        {
            RuleFor(x => x.ShipperId)
                .GreaterThan(0).WithMessage("Shipper ID must be greater than 0.");
        }
    }
}