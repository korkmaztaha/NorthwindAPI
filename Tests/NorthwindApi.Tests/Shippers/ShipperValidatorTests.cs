using FluentValidation.TestHelper;
using NorthwindApi.Application.Features.Shippers.Commands.CreateShipper;
using NorthwindApi.Application.Features.Shippers.Commands.UpdateShipper;
using NorthwindApi.Application.Features.Shippers.Commands.DeleteShipper;
using NorthwindApi.Application.Features.Shippers.Queries.GetShippers;

namespace NorthwindApi.Tests.Shippers;

    public class ShipperValidatorTests
    {
        private readonly CreateShipperCommandValidator _createValidator = new();
        private readonly UpdateShipperCommandValidator _updateValidator = new();
        private readonly DeleteShipperCommandValidator _deleteValidator = new();
        private readonly GetShippersQueryValidator _getValidator = new();

        // ───────────────── CREATE ─────────────────

        [Fact]
        public void CreateShipper_ShouldNotHaveError_WhenValidRequest()
        {
            var command = new CreateShipperCommand
            {
                CompanyName = "FastShip",
                Phone = "111-222"
            };

            var result = _createValidator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void CreateShipper_ShouldHaveError_WhenCompanyNameIsInvalid(string companyName)
        {
            var command = new CreateShipperCommand
            {
                CompanyName = companyName
            };

            var result = _createValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CompanyName)
                .WithErrorMessage("Company name is required.");
        }

        [Fact]
        public void CreateShipper_ShouldHaveError_WhenCompanyNameExceedsMaxLength()
        {
            var command = new CreateShipperCommand
            {
                CompanyName = new string('A', 41)
            };

            var result = _createValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CompanyName)
                .WithErrorMessage("Company name must not exceed 40 characters.");
        }

        // ───────────────── UPDATE ─────────────────

        [Fact]
        public void UpdateShipper_ShouldNotHaveError_WhenValidRequest()
        {
            var command = new UpdateShipperCommand
            {
                ShipperId = 1,
                CompanyName = "NewShip",
                Phone = "000"
            };

            var result = _updateValidator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void UpdateShipper_ShouldHaveError_WhenShipperIdIsInvalid(int shipperId)
        {
            var command = new UpdateShipperCommand
            {
                ShipperId = shipperId,
                CompanyName = "ValidName"
            };

            var result = _updateValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ShipperId)
                .WithErrorMessage("Shipper ID must be greater than 0.");
        }

        // ───────────────── DELETE ─────────────────

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void DeleteShipper_ShouldHaveError_WhenShipperIdIsInvalid(int shipperId)
        {
            var command = new DeleteShipperCommand
            {
                ShipperId = shipperId
            };

            var result = _deleteValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ShipperId)
                .WithErrorMessage("Shipper ID must be greater than 0.");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(999)]
        public void DeleteShipper_ShouldNotHaveError_WhenShipperIdIsValid(int shipperId)
        {
            var command = new DeleteShipperCommand
            {
                ShipperId = shipperId
            };

            var result = _deleteValidator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.ShipperId);
        }

        // ───────────────── GET ─────────────────

        [Fact]
        public void GetShippers_ShouldNotHaveError_WhenValidRequest()
        {
            var query = new GetShippersQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            var result = _getValidator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void GetShippers_ShouldHaveError_WhenPageNumberIsInvalid(int pageNumber)
        {
            var query = new GetShippersQuery
            {
                PageNumber = pageNumber,
                PageSize = 10
            };

            var result = _getValidator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.PageNumber)
                .WithErrorMessage("Page number must be greater than 0.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        [InlineData(-1)]
        public void GetShippers_ShouldHaveError_WhenPageSizeIsInvalid(int pageSize)
        {
            var query = new GetShippersQuery
            {
                PageNumber = 1,
                PageSize = pageSize
            };

            var result = _getValidator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.PageSize)
                .WithErrorMessage("Page size must be between 1 and 100.");
        }
    }