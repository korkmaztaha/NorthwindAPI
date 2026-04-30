using FluentValidation.TestHelper;
using NorthwindApi.Application.Features.Customers.Commands.CreateCustomer;
using NorthwindApi.Application.Features.Customers.Commands.DeleteCustomer;
using NorthwindApi.Application.Features.Customers.Commands.UpdateCustomer;
using NorthwindApi.Application.Features.Customers.Queries.GetCustomers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Tests.Customers
{


    public class CustomerValidatorTests
    {
        private readonly CreateCustomerCommandValidator _createValidator;
        private readonly UpdateCustomerCommandValidator _updateValidator;
        private readonly DeleteCustomerCommandValidator _deleteValidator;
        private readonly GetCustomersQueryValidator _getValidator;

        public CustomerValidatorTests()
        {
            _createValidator = new CreateCustomerCommandValidator();
            _updateValidator = new UpdateCustomerCommandValidator();
            _deleteValidator = new DeleteCustomerCommandValidator();
            _getValidator = new GetCustomersQueryValidator();
        }

        // ───────────── CreateCustomerCommand Tests ─────────────

        [Fact]
        public void CreateCustomer_ShouldNotHaveError_WhenValidRequest()
        {
            // Arrange
            var command = new CreateCustomerCommand
            {
                CustomerId = "TESTX",
                CompanyName = "Test Company",
                ContactName = "John Doe",
                Country = "Turkey"
            };

            // Act
            var result = _createValidator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void CreateCustomer_ShouldHaveError_WhenCustomerIdIsEmpty()
        {
            // Arrange
            var command = new CreateCustomerCommand
            {
                CustomerId = "",
                CompanyName = "Test Company"
            };

            // Act
            var result = _createValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CustomerId);
        }

        [Fact]
        public void CreateCustomer_ShouldHaveError_WhenCustomerIdIsNot5Characters()
        {
            // Arrange
            var command = new CreateCustomerCommand
            {
                CustomerId = "TST",
                CompanyName = "Test Company"
            };

            // Act
            var result = _createValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CustomerId)
                .WithErrorMessage("Customer ID must be exactly 5 characters.");
        }

        [Fact]
        public void CreateCustomer_ShouldHaveError_WhenCustomerIdContainsLowerCase()
        {
            // Arrange
            var command = new CreateCustomerCommand
            {
                CustomerId = "testx",
                CompanyName = "Test Company"
            };

            // Act
            var result = _createValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CustomerId)
                .WithErrorMessage("Customer ID must contain only uppercase letters.");
        }

        [Fact]
        public void CreateCustomer_ShouldHaveError_WhenCompanyNameIsEmpty()
        {
            // Arrange
            var command = new CreateCustomerCommand
            {
                CustomerId = "TESTX",
                CompanyName = ""
            };

            // Act
            var result = _createValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CompanyName)
                .WithErrorMessage("Company name is required.");
        }

        [Fact]
        public void CreateCustomer_ShouldHaveError_WhenCompanyNameExceedsMaxLength()
        {
            // Arrange
            var command = new CreateCustomerCommand
            {
                CustomerId = "TESTX",
                CompanyName = new string('A', 41) // 41 karakter
            };

            // Act
            var result = _createValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CompanyName)
                .WithErrorMessage("Company name must not exceed 40 characters.");
        }

        [Fact]
        public void CreateCustomer_ShouldHaveError_WhenCountryExceedsMaxLength()
        {
            // Arrange
            var command = new CreateCustomerCommand
            {
                CustomerId = "TESTX",
                CompanyName = "Test Company",
                Country = new string('A', 16) // 16 karakter
            };

            // Act
            var result = _createValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Country)
                .WithErrorMessage("Country must not exceed 15 characters.");
        }

        [Theory]
        [InlineData("TEST1")]  // rakam içeriyor
        [InlineData("testx")]  // küçük harf
        [InlineData("TES")]    // 3 karakter
        [InlineData("TESTXX")] // 6 karakter
        [InlineData("")]       // boş
        public void CreateCustomer_ShouldHaveError_WhenCustomerIdIsInvalid(string customerId)
        {
            // Arrange
            var command = new CreateCustomerCommand
            {
                CustomerId = customerId,
                CompanyName = "Test Company"
            };

            // Act
            var result = _createValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CustomerId);
        }

        [Theory]
        [InlineData("TESTX")]
        [InlineData("ABCDE")]
        [InlineData("ZXCVB")]
        public void CreateCustomer_ShouldNotHaveError_WhenCustomerIdIsValid(string customerId)
        {
            // Arrange
            var command = new CreateCustomerCommand
            {
                CustomerId = customerId,
                CompanyName = "Test Company"
            };

            // Act
            var result = _createValidator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.CustomerId);
        }

        // ───────────── UpdateCustomerCommand Tests ─────────────

        [Fact]
        public void UpdateCustomer_ShouldNotHaveError_WhenValidRequest()
        {
            // Arrange
            var command = new UpdateCustomerCommand
            {
                CustomerId = "TESTX",
                CompanyName = "Updated Company"
            };

            // Act
            var result = _updateValidator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void UpdateCustomer_ShouldHaveError_WhenCustomerIdIsEmpty()
        {
            // Arrange
            var command = new UpdateCustomerCommand
            {
                CustomerId = "",
                CompanyName = "Test Company"
            };

            // Act
            var result = _updateValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CustomerId);
        }

        [Fact]
        public void UpdateCustomer_ShouldHaveError_WhenCompanyNameIsEmpty()
        {
            // Arrange
            var command = new UpdateCustomerCommand
            {
                CustomerId = "TESTX",
                CompanyName = ""
            };

            // Act
            var result = _updateValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CompanyName)
                .WithErrorMessage("Company name is required.");
        }

        // ───────────── DeleteCustomerCommand Tests ─────────────

        [Fact]
        public void DeleteCustomer_ShouldNotHaveError_WhenValidRequest()
        {
            // Arrange
            var command = new DeleteCustomerCommand { CustomerId = "TESTX" };

            // Act
            var result = _deleteValidator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void DeleteCustomer_ShouldHaveError_WhenCustomerIdIsEmpty()
        {
            // Arrange
            var command = new DeleteCustomerCommand { CustomerId = "" };

            // Act
            var result = _deleteValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.CustomerId);
        }

        // ───────────── GetCustomersQuery Tests ─────────────

        [Fact]
        public void GetCustomers_ShouldNotHaveError_WhenValidRequest()
        {
            // Arrange
            var query = new GetCustomersQuery { PageNumber = 1, PageSize = 10 };

            // Act
            var result = _getValidator.TestValidate(query);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void GetCustomers_ShouldHaveError_WhenPageNumberIsInvalid(int pageNumber)
        {
            // Arrange
            var query = new GetCustomersQuery { PageNumber = pageNumber, PageSize = 10 };

            // Act
            var result = _getValidator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PageNumber)
                .WithErrorMessage("Page number must be greater than 0.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        [InlineData(-1)]
        public void GetCustomers_ShouldHaveError_WhenPageSizeIsInvalid(int pageSize)
        {
            // Arrange
            var query = new GetCustomersQuery { PageNumber = 1, PageSize = pageSize };

            // Act
            var result = _getValidator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.PageSize)
                .WithErrorMessage("Page size must be between 1 and 100.");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        public void GetCustomers_ShouldNotHaveError_WhenPageSizeIsValid(int pageSize)
        {
            // Arrange
            var query = new GetCustomersQuery { PageNumber = 1, PageSize = pageSize };

            // Act
            var result = _getValidator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
        }
    }
}