using FluentValidation.TestHelper;
using NorthwindApi.Application.Features.Shippers.Commands.CreateShipper;
using NorthwindApi.Application.Features.Shippers.Commands.DeleteShipper;
using NorthwindApi.Application.Features.Shippers.Commands.UpdateShipper;
using NorthwindApi.Application.Features.Shippers.Queries.GetShippers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Tests.Shippers
{
    public class ShipperValidatorTests
    {
        private readonly CreateShipperCommandValidator _createValidator;
        private readonly UpdateShipperCommandValidator _updateValidator;
        private readonly DeleteShipperCommandValidator _deleteValidator;
        private readonly GetShippersQueryValidator _getValidator;

        public ShipperValidatorTests()
        {
            _createValidator = new CreateShipperCommandValidator();
            _updateValidator = new UpdateShipperCommandValidator();
            _deleteValidator = new DeleteShipperCommandValidator();
            _getValidator = new GetShippersQueryValidator();
        }

        [Fact]
        public void CreateShipper_ShouldNotHaveError_WhenValidRequest()
        {
            var command = new CreateShipperCommand { CompanyName = "FastShip", Phone = "111-222" };
            var result = _createValidator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void CreateShipper_ShouldHaveError_WhenCompanyNameIsEmpty()
        {
            var command = new CreateShipperCommand { CompanyName = "" };
            var result = _createValidator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.CompanyName)
                .WithErrorMessage("Company name is required.");
        }

        [Fact]
        public void CreateShipper_ShouldHaveError_WhenCompanyNameExceedsMaxLength()
        {
            var command = new CreateShipperCommand { CompanyName = new string('A', 41) };
            var result = _createValidator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.CompanyName)
                .WithErrorMessage("Company name must not exceed 40 characters.");
        }

        [Fact]
        public void UpdateShipper_ShouldNotHaveError_WhenValidRequest()
        {
            var command = new UpdateShipperCommand { ShipperId = 1, CompanyName = "NewShip", Phone = "000" };
            var result = _updateValidator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void UpdateShipper_ShouldHaveError_WhenShipperIdIsZero()
        {
            var command = new UpdateShipperCommand { ShipperId = 0, CompanyName = "X" };
            var result = _updateValidator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ShipperId)
                .WithErrorMessage("Shipper ID must be greater than 0.");
        }

        [Fact]
        public void DeleteShipper_ShouldHaveError_WhenShipperIdIsNegative()
        {
            var command = new DeleteShipperCommand { ShipperId = -1 };
            var result = _deleteValidator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.ShipperId)
                .WithErrorMessage("Shipper ID must be greater than 0.");
        }

        [Fact]
        public void GetShippers_ShouldNotHaveError_WhenValidRequest()
        {
            var query = new GetShippersQuery { PageNumber = 1, PageSize = 10 };
            var result = _getValidator.TestValidate(query);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
