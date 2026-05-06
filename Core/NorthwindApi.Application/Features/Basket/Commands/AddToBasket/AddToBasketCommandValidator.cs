using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Basket.Commands.AddToBasket
{
    public class AddToBasketCommandValidator : AbstractValidator<AddToBasketCommand>
    {
        public AddToBasketCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer ID is required.")
                .Length(5).WithMessage("Customer ID must be exactly 5 characters.")
                .Matches("^[A-Z]+$").WithMessage("Customer ID must contain only uppercase letters.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Product ID must be greater than 0.");

            RuleFor(x => x.Quantity)
                .GreaterThan((short)0).WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.Discount)
                .InclusiveBetween(0, 1).WithMessage("Discount must be between 0 and 1.");
        }
    }
}
