using FluentValidation.TestHelper;
using NorthwindApi.Application.Features.Products.Commands.CreateProduct;
using NorthwindApi.Application.Features.Products.Commands.DeleteProduct;
using NorthwindApi.Application.Features.Products.Commands.UpdateProduct;
using NorthwindApi.Application.Features.Products.Queries.GetProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Tests.Products
{
    public class ProductValidatorTests
    {
        private readonly CreateProductCommandValidator _createValidator;
        private readonly UpdateProductCommandValidator _updateValidator;
        private readonly DeleteProductCommandValidator _deleteValidator;
        private readonly GetProductsQueryValidator _getValidator;

        public ProductValidatorTests()
        {
            _createValidator = new CreateProductCommandValidator();
            _updateValidator = new UpdateProductCommandValidator();
            _deleteValidator = new DeleteProductCommandValidator();
            _getValidator = new GetProductsQueryValidator();
        }

        // ───────────── CreateProductCommand Tests ─────────────

        [Fact]
        public void CreateProduct_ShouldNotHaveError_WhenValidRequest()
        {
            // Arrange
            var command = new CreateProductCommand
            {
                ProductName = "Test Product",
                UnitPrice = 99.99m,
                UnitsInStock = 100,
                CategoryId = 1,
                SupplierId = 1,
                QuantityPerUnit = "10 boxes",
                UnitsOnOrder = 0,
                ReorderLevel = 10,
                Discontinued = false
            };

            // Act
            var result = _createValidator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        public void CreateProduct_ShouldHaveError_WhenProductNameIsEmpty(string productName)
        {
            // Arrange
            var command = new CreateProductCommand
            {
                ProductName = productName,
                UnitPrice = 99.99m
            };

            // Act
            var result = _createValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProductName)
                .WithErrorMessage("Product name is required.");
        }

        [Fact]
        public void CreateProduct_ShouldHaveError_WhenProductNameExceedsMaxLength()
        {
            // Arrange
            var command = new CreateProductCommand
            {
                ProductName = new string('A', 41), // 41 karakter
                UnitPrice = 99.99m
            };

            // Act
            var result = _createValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProductName)
                .WithErrorMessage("Product name must not exceed 40 characters.");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100.5)]
        public void CreateProduct_ShouldHaveError_WhenUnitPriceIsNegative(decimal unitPrice)
        {
            // Arrange
            var command = new CreateProductCommand
            {
                ProductName = "Test Product",
                UnitPrice = unitPrice
            };

            // Act
            var result = _createValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UnitPrice)
                .WithErrorMessage("Unit price must be greater than or equal to 0.");
        }

        [Fact]
        public void CreateProduct_ShouldHaveError_WhenUnitsInStockIsNegative()
        {
            // Arrange
            var command = new CreateProductCommand
            {
                ProductName = "Test Product",
                UnitPrice = 99.99m,
                UnitsInStock = -10
            };

            // Act
            var result = _createValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UnitsInStock)
                .WithErrorMessage("Units in stock must be greater than or equal to 0.");
        }

        [Fact]
        public void CreateProduct_ShouldHaveError_WhenQuantityPerUnitExceedsMaxLength()
        {
            // Arrange
            var command = new CreateProductCommand
            {
                ProductName = "Test Product",
                UnitPrice = 99.99m,
                QuantityPerUnit = new string('A', 21) // 21 karakter
            };

            // Act
            var result = _createValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.QuantityPerUnit)
                .WithErrorMessage("Quantity per unit must not exceed 20 characters.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(99.99)]
        [InlineData(1000)]
        public void CreateProduct_ShouldNotHaveError_WhenUnitPriceIsValid(decimal unitPrice)
        {
            // Arrange
            var command = new CreateProductCommand
            {
                ProductName = "Test Product",
                UnitPrice = unitPrice
            };

            // Act
            var result = _createValidator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.UnitPrice);
        }

        // ───────────── UpdateProductCommand Tests ─────────────

        [Fact]
        public void UpdateProduct_ShouldNotHaveError_WhenValidRequest()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                ProductId = 1,
                ProductName = "Updated Product",
                UnitPrice = 149.99m,
                UnitsInStock = 50,
                CategoryId = 1,
                SupplierId = 1
            };

            // Act
            var result = _updateValidator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void UpdateProduct_ShouldHaveError_WhenProductIdIsZero()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                ProductId = 0,
                ProductName = "Test Product"
            };

            // Act
            var result = _updateValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProductId)
                .WithErrorMessage("Product ID must be greater than 0.");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void UpdateProduct_ShouldHaveError_WhenProductIdIsNegative(int productId)
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                ProductId = productId,
                ProductName = "Test Product"
            };

            // Act
            var result = _updateValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProductId)
                .WithErrorMessage("Product ID must be greater than 0.");
        }

        [Fact]
        public void UpdateProduct_ShouldHaveError_WhenProductNameIsEmpty()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                ProductId = 1,
                ProductName = ""
            };

            // Act
            var result = _updateValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProductName)
                .WithErrorMessage("Product name is required.");
        }

        [Fact]
        public void UpdateProduct_ShouldHaveError_WhenProductNameExceedsMaxLength()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                ProductId = 1,
                ProductName = new string('A', 41) // 41 karakter
            };

            // Act
            var result = _updateValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProductName)
                .WithErrorMessage("Product name must not exceed 40 characters.");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100.5)]
        public void UpdateProduct_ShouldHaveError_WhenUnitPriceIsNegative(decimal unitPrice)
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                ProductId = 1,
                ProductName = "Test Product",
                UnitPrice = unitPrice
            };

            // Act
            var result = _updateValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UnitPrice)
                .WithErrorMessage("Unit price must be greater than or equal to 0.");
        }

        // ───────────── DeleteProductCommand Tests ─────────────

        [Fact]
        public void DeleteProduct_ShouldNotHaveError_WhenValidRequest()
        {
            // Arrange
            var command = new DeleteProductCommand { ProductId = 1 };

            // Act
            var result = _deleteValidator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void DeleteProduct_ShouldHaveError_WhenProductIdIsZero()
        {
            // Arrange
            var command = new DeleteProductCommand { ProductId = 0 };

            // Act
            var result = _deleteValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProductId)
                .WithErrorMessage("Product ID must be greater than 0.");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void DeleteProduct_ShouldHaveError_WhenProductIdIsNegative(int productId)
        {
            // Arrange
            var command = new DeleteProductCommand { ProductId = productId };

            // Act
            var result = _deleteValidator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ProductId)
                .WithErrorMessage("Product ID must be greater than 0.");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(9999)]
        public void DeleteProduct_ShouldNotHaveError_WhenProductIdIsValid(int productId)
        {
            // Arrange
            var command = new DeleteProductCommand { ProductId = productId };

            // Act
            var result = _deleteValidator.TestValidate(command);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.ProductId);
        }

        // ───────────── GetProductsQuery Tests ─────────────

        [Fact]
        public void GetProducts_ShouldNotHaveError_WhenValidRequest()
        {
            // Arrange
            var query = new GetProductsQuery { PageNumber = 1, PageSize = 10 };

            // Act
            var result = _getValidator.TestValidate(query);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void GetProducts_ShouldHaveError_WhenPageNumberIsInvalid(int pageNumber)
        {
            // Arrange
            var query = new GetProductsQuery { PageNumber = pageNumber, PageSize = 10 };

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
        public void GetProducts_ShouldHaveError_WhenPageSizeIsInvalid(int pageSize)
        {
            // Arrange
            var query = new GetProductsQuery { PageNumber = 1, PageSize = pageSize };

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
        public void GetProducts_ShouldNotHaveError_WhenPageSizeIsValid(int pageSize)
        {
            // Arrange
            var query = new GetProductsQuery { PageNumber = 1, PageSize = pageSize };

            // Act
            var result = _getValidator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100.5)]
        public void GetProducts_ShouldHaveError_WhenMinPriceIsNegative(decimal minPrice)
        {
            // Arrange
            var query = new GetProductsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                MinPrice = minPrice
            };

            // Act
            var result = _getValidator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.MinPrice)
                .WithErrorMessage("Min price must be greater than or equal to 0.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(99.99)]
        [InlineData(1000)]
        public void GetProducts_ShouldNotHaveError_WhenMinPriceIsValid(decimal minPrice)
        {
            // Arrange
            var query = new GetProductsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                MinPrice = minPrice
            };

            // Act
            var result = _getValidator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.MinPrice);
        }

        [Fact]
        public void GetProducts_ShouldHaveError_WhenMaxPriceIsLessThanMinPrice()
        {
            // Arrange
            var query = new GetProductsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                MinPrice = 100m,
                MaxPrice = 50m
            };

            // Act
            var result = _getValidator.TestValidate(query);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.MaxPrice)
                .WithErrorMessage("Max price must be greater than min price.");
        }

        [Theory]
        [InlineData(0, 100)]
        [InlineData(50, 100)]
        [InlineData(99.99, 199.99)]
        public void GetProducts_ShouldNotHaveError_WhenPriceRangeIsValid(decimal minPrice, decimal maxPrice)
        {
            // Arrange
            var query = new GetProductsQuery
            {
                PageNumber = 1,
                PageSize = 10,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };

            // Act
            var result = _getValidator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.MaxPrice);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        public void GetProducts_ShouldNotHaveError_WhenPageNumberIsValid(int pageNumber)
        {
            // Arrange
            var query = new GetProductsQuery { PageNumber = pageNumber, PageSize = 10 };

            // Act
            var result = _getValidator.TestValidate(query);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
        }
    }
}
