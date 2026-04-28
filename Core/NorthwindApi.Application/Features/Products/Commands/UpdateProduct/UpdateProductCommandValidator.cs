using FluentValidation;
using NorthwindApi.Application.Features.Products.Commands.UpdateProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Product ID must be greater than 0.");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(40).WithMessage("Product name must not exceed 40 characters.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Unit price must be greater than or equal to 0.")
            .When(x => x.UnitPrice.HasValue);

        RuleFor(x => x.UnitsInStock)
            .GreaterThanOrEqualTo((short)0).WithMessage("Units in stock must be greater than or equal to 0.")
            .When(x => x.UnitsInStock.HasValue);

        RuleFor(x => x.UnitsOnOrder)
            .GreaterThanOrEqualTo((short)0).WithMessage("Units on order must be greater than or equal to 0.")
            .When(x => x.UnitsOnOrder.HasValue);

        RuleFor(x => x.ReorderLevel)
            .GreaterThanOrEqualTo((short)0).WithMessage("Reorder level must be greater than or equal to 0.")
            .When(x => x.ReorderLevel.HasValue);

        RuleFor(x => x.QuantityPerUnit)
            .MaximumLength(20).WithMessage("Quantity per unit must not exceed 20 characters.")
            .When(x => x.QuantityPerUnit != null);
    }
}