using FluentValidation.TestHelper;
using NorthwindApi.Application.Features.Employees.Commands.CreateEmployee;
using NorthwindApi.Application.Features.Employees.Commands.DeleteEmployee;
using NorthwindApi.Application.Features.Employees.Commands.UpdateEmployee;
using NorthwindApi.Application.Features.Employees.Queries.GetEmployees;

namespace NorthwindApi.Tests.Employees;

public class EmployeeValidatorTests
{
    private readonly CreateEmployeeCommandValidator _createValidator = new();
    private readonly UpdateEmployeeCommandValidator _updateValidator = new();
    private readonly DeleteEmployeeCommandValidator _deleteValidator = new();

    // ───────────── CreateEmployeeCommand Tests ─────────────

    [Fact]
    public void CreateEmployee_ShouldNotHaveError_WhenValidRequest()
    {
        var command = new CreateEmployeeCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Title = "Sales Representative",
            HireDate = DateTime.UtcNow.AddDays(-1),
            BirthDate = DateTime.UtcNow.AddYears(-30),
            ReportsTo = 2
        };

        var result = _createValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CreateEmployee_ShouldHaveError_WhenFirstNameIsEmpty(string firstName)
    {
        var command = new CreateEmployeeCommand
        {
            FirstName = firstName,
            LastName = "Doe"
        };

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name is required.");
    }

    [Fact]
    public void CreateEmployee_ShouldHaveError_WhenFirstNameExceedsMaxLength()
    {
        var command = new CreateEmployeeCommand
        {
            FirstName = new string('A', 11),
            LastName = "Doe"
        };

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name must not exceed 10 characters.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void CreateEmployee_ShouldHaveError_WhenLastNameIsEmpty(string lastName)
    {
        var command = new CreateEmployeeCommand
        {
            FirstName = "John",
            LastName = lastName
        };

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required.");
    }

    [Fact]
    public void CreateEmployee_ShouldHaveError_WhenLastNameExceedsMaxLength()
    {
        var command = new CreateEmployeeCommand
        {
            FirstName = "John",
            LastName = new string('A', 21)
        };

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name must not exceed 20 characters.");
    }

    [Fact]
    public void CreateEmployee_ShouldHaveError_WhenBirthDateIsInFuture()
    {
        var command = new CreateEmployeeCommand
        {
            FirstName = "John",
            LastName = "Doe",
            BirthDate = DateTime.UtcNow.AddDays(1)
        };

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.BirthDate)
            .WithErrorMessage("Birth date cannot be in the future.");
    }

    [Fact]
    public void CreateEmployee_ShouldHaveError_WhenHireDateIsInFuture()
    {
        var command = new CreateEmployeeCommand
        {
            FirstName = "John",
            LastName = "Doe",
            HireDate = DateTime.UtcNow.AddDays(1)
        };

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.HireDate)
            .WithErrorMessage("Hire date cannot be in the future.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CreateEmployee_ShouldHaveError_WhenReportsToIsInvalid(int reportsTo)
    {
        var command = new CreateEmployeeCommand
        {
            FirstName = "John",
            LastName = "Doe",
            ReportsTo = reportsTo
        };

        var result = _createValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.ReportsTo)
            .WithErrorMessage("Reports to must be greater than 0.");
    }

    [Fact]
    public void CreateEmployee_ShouldNotHaveError_WhenReportsToIsNull()
    {
        var command = new CreateEmployeeCommand
        {
            FirstName = "John",
            LastName = "Doe",
            ReportsTo = null
        };

        var result = _createValidator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.ReportsTo);
    }

    // ───────────── UpdateEmployeeCommand Tests ─────────────

    [Fact]
    public void UpdateEmployee_ShouldNotHaveError_WhenValidRequest()
    {
        var command = new UpdateEmployeeCommand
        {
            EmployeeId = 1,
            FirstName = "John",
            LastName = "Doe"
        };

        var result = _updateValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void UpdateEmployee_ShouldHaveError_WhenEmployeeIdIsInvalid(int employeeId)
    {
        var command = new UpdateEmployeeCommand
        {
            EmployeeId = employeeId,
            FirstName = "John",
            LastName = "Doe"
        };

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EmployeeId)
            .WithErrorMessage("Employee ID must be greater than 0.");
    }

    [Fact]
    public void UpdateEmployee_ShouldHaveError_WhenFirstNameIsEmpty()
    {
        var command = new UpdateEmployeeCommand
        {
            EmployeeId = 1,
            FirstName = "",
            LastName = "Doe"
        };

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("First name is required.");
    }

    [Fact]
    public void UpdateEmployee_ShouldHaveError_WhenLastNameIsEmpty()
    {
        var command = new UpdateEmployeeCommand
        {
            EmployeeId = 1,
            FirstName = "John",
            LastName = ""
        };

        var result = _updateValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("Last name is required.");
    }

    // ───────────── DeleteEmployeeCommand Tests ─────────────

    [Fact]
    public void DeleteEmployee_ShouldNotHaveError_WhenValidRequest()
    {
        var command = new DeleteEmployeeCommand { EmployeeId = 1 };

        var result = _deleteValidator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void DeleteEmployee_ShouldHaveError_WhenEmployeeIdIsInvalid(int employeeId)
    {
        var command = new DeleteEmployeeCommand { EmployeeId = employeeId };

        var result = _deleteValidator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(x => x.EmployeeId)
            .WithErrorMessage("Employee ID must be greater than 0.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(999)]
    public void DeleteEmployee_ShouldNotHaveError_WhenEmployeeIdIsValid(int employeeId)
    {
        var command = new DeleteEmployeeCommand { EmployeeId = employeeId };

        var result = _deleteValidator.TestValidate(command);

        result.ShouldNotHaveValidationErrorFor(x => x.EmployeeId);
    }
}