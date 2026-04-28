using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Employees.Commands.UpdateEmployee
{
    public class UpdateEmployeeCommandValidator : AbstractValidator<UpdateEmployeeCommand>
    {
        public UpdateEmployeeCommandValidator()
        {
            RuleFor(x => x.EmployeeId)
                .GreaterThan(0).WithMessage("Employee ID must be greater than 0.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(10).WithMessage("First name must not exceed 10 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(20).WithMessage("Last name must not exceed 20 characters.");

            RuleFor(x => x.Title)
                .MaximumLength(30).WithMessage("Title must not exceed 30 characters.")
                .When(x => x.Title != null);

            RuleFor(x => x.BirthDate)
                .LessThan(DateTime.UtcNow).WithMessage("Birth date cannot be in the future.")
                .When(x => x.BirthDate.HasValue);

            RuleFor(x => x.HireDate)
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Hire date cannot be in the future.")
                .When(x => x.HireDate.HasValue);

            RuleFor(x => x.ReportsTo)
                .GreaterThan(0).WithMessage("Reports to must be greater than 0.")
                .When(x => x.ReportsTo.HasValue);
        }
    }
}