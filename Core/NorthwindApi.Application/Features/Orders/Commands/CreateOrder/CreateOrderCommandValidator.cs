using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer ID is required.")
                .Length(5).WithMessage("Customer ID must be exactly 5 characters.");

            RuleFor(x => x.Freight)
                .GreaterThanOrEqualTo(0).WithMessage("Freight must be greater than or equal to 0.")
                .When(x => x.Freight.HasValue);

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must have at least one item.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(x => x.ProductId)
                    .GreaterThan(0).WithMessage("Product ID must be greater than 0.");

                item.RuleFor(x => x.Quantity)
                    .GreaterThan((short)0).WithMessage("Quantity must be greater than 0.");

                item.RuleFor(x => x.Discount)
                    .InclusiveBetween(0, 1).WithMessage("Discount must be between 0 and 1.");
            });
        }
    }
}