using NorthwindApi.Application.Features.Basket.Commands.AddToBasket;
using NorthwindApi.Application.Features.Basket.Commands.ClearBasket;
using NorthwindApi.Application.Features.Basket.Commands.RemoveFromBasket;
using NorthwindApi.Application.Features.Basket.Queries.GetBasket;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Tests.Baskets
{
    public class BasketValidatorTests
    {
        private readonly AddToBasketCommandValidator _addValidator = new();
        private readonly RemoveFromBasketCommandValidator _removeValidator = new();
        private readonly ClearBasketCommandValidator _clearValidator = new();
        private readonly GetBasketQueryValidator _getValidator = new();

        // ───────────── AddToBasket Tests ─────────────

        [Fact]
        public void AddToBasket_ShouldNotHaveError_WhenValidRequest()
        {
            var command = new AddToBasketCommand
            {
                CustomerId = "ALFKI",
                ProductId = 1,
                Quantity = 3,
                Discount = 0
            };

            var result = _addValidator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData("alfki")]
        [InlineData("TST")]
        [InlineData("TESTXX")]
        public void AddToBasket_ShouldHaveError_WhenCustomerIdIsInvalid(string customerId)
        {
            var command = new AddToBasketCommand
            {
                CustomerId = customerId,
                ProductId = 1,
                Quantity = 3,
                Discount = 0
            };

            var result = _addValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CustomerId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AddToBasket_ShouldHaveError_WhenProductIdIsInvalid(int productId)
        {
            var command = new AddToBasketCommand
            {
                CustomerId = "ALFKI",
                ProductId = productId,
                Quantity = 3,
                Discount = 0
            };

            var result = _addValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ProductId)
                .WithErrorMessage("Product ID must be greater than 0.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void AddToBasket_ShouldHaveError_WhenQuantityIsInvalid(short quantity)
        {
            var command = new AddToBasketCommand
            {
                CustomerId = "ALFKI",
                ProductId = 1,
                Quantity = quantity,
                Discount = 0
            };

            var result = _addValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Quantity)
                .WithErrorMessage("Quantity must be greater than 0.");
        }

        [Theory]
        [InlineData(-0.1f)]
        [InlineData(1.1f)]
        [InlineData(2f)]
        public void AddToBasket_ShouldHaveError_WhenDiscountIsInvalid(float discount)
        {
            var command = new AddToBasketCommand
            {
                CustomerId = "ALFKI",
                ProductId = 1,
                Quantity = 3,
                Discount = discount
            };

            var result = _addValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Discount)
                .WithErrorMessage("Discount must be between 0 and 1.");
        }

        [Theory]
        [InlineData(0f)]
        [InlineData(0.5f)]
        [InlineData(1f)]
        public void AddToBasket_ShouldNotHaveError_WhenDiscountIsValid(float discount)
        {
            var command = new AddToBasketCommand
            {
                CustomerId = "ALFKI",
                ProductId = 1,
                Quantity = 3,
                Discount = discount
            };

            var result = _addValidator.TestValidate(command);

            result.ShouldNotHaveValidationErrorFor(x => x.Discount);
        }

        // ───────────── RemoveFromBasket Tests ─────────────

        [Fact]
        public void RemoveFromBasket_ShouldNotHaveError_WhenValidRequest()
        {
            var command = new RemoveFromBasketCommand
            {
                CustomerId = "ALFKI",
                ProductId = 1
            };

            var result = _removeValidator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData("alfki")]
        [InlineData("TST")]
        public void RemoveFromBasket_ShouldHaveError_WhenCustomerIdIsInvalid(string customerId)
        {
            var command = new RemoveFromBasketCommand
            {
                CustomerId = customerId,
                ProductId = 1
            };

            var result = _removeValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CustomerId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void RemoveFromBasket_ShouldHaveError_WhenProductIdIsInvalid(int productId)
        {
            var command = new RemoveFromBasketCommand
            {
                CustomerId = "ALFKI",
                ProductId = productId
            };

            var result = _removeValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.ProductId)
                .WithErrorMessage("Product ID must be greater than 0.");
        }

        // ───────────── ClearBasket Tests ─────────────

        [Fact]
        public void ClearBasket_ShouldNotHaveError_WhenValidRequest()
        {
            var command = new ClearBasketCommand { CustomerId = "ALFKI" };

            var result = _clearValidator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData("alfki")]
        [InlineData("TST")]
        public void ClearBasket_ShouldHaveError_WhenCustomerIdIsInvalid(string customerId)
        {
            var command = new ClearBasketCommand { CustomerId = customerId };

            var result = _clearValidator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.CustomerId);
        }

        // ───────────── GetBasket Tests ─────────────

        [Fact]
        public void GetBasket_ShouldNotHaveError_WhenValidRequest()
        {
            var query = new GetBasketQuery { CustomerId = "ALFKI" };

            var result = _getValidator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData("alfki")]
        [InlineData("TST")]
        [InlineData("TESTXX")]
        public void GetBasket_ShouldHaveError_WhenCustomerIdIsInvalid(string customerId)
        {
            var query = new GetBasketQuery { CustomerId = customerId };

            var result = _getValidator.TestValidate(query);

            result.ShouldHaveValidationErrorFor(x => x.CustomerId);
        }
    }
}