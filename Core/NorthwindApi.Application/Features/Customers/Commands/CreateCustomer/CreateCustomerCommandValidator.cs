using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("Customer ID is required.")
                .Length(5).WithMessage("Customer ID must be exactly 5 characters.")
                .Matches("^[A-Z]+$").WithMessage("Customer ID must contain only uppercase letters.");

            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(40).WithMessage("Company name must not exceed 40 characters.");

            RuleFor(x => x.ContactName)
                .MaximumLength(30).WithMessage("Contact name must not exceed 30 characters.")
                .When(x => x.ContactName != null);

            RuleFor(x => x.ContactTitle)
                .MaximumLength(30).WithMessage("Contact title must not exceed 30 characters.")
                .When(x => x.ContactTitle != null);

            RuleFor(x => x.Address)
                .MaximumLength(60).WithMessage("Address must not exceed 60 characters.")
                .When(x => x.Address != null);

            RuleFor(x => x.City)
                .MaximumLength(15).WithMessage("City must not exceed 15 characters.")
                .When(x => x.City != null);

            RuleFor(x => x.Country)
                .MaximumLength(15).WithMessage("Country must not exceed 15 characters.")
                .When(x => x.Country != null);

            RuleFor(x => x.Phone)
                .MaximumLength(24).WithMessage("Phone must not exceed 24 characters.")
                .When(x => x.Phone != null);

           
        }
    }
}
