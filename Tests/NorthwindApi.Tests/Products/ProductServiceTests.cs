using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using NorthwindApi.Application.Features.Products.Commands.CreateProduct;
using NorthwindApi.Application.Features.Products.Commands.UpdateProduct;
using NorthwindApi.Application.Features.Products.Queries.GetProducts;
using NorthwindApi.Application.Interfaces.BusinessRules;
using NorthwindApi.Domain.Entities;
using NorthwindApi.Persistence.Services.EntityServices;
using NorthwindApi.Tests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductEntity = NorthwindApi.Domain.Entities.Products;

namespace NorthwindApi.Tests.Products
{
    public class ProductServiceTests : TestBase
    {
        private readonly Mock<IProductBusinessRules> _mockBusinessRules;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockBusinessRules = new Mock<IProductBusinessRules>();
            _productService = new ProductService(
                MockUnitOfWork.Object, 
                _mockBusinessRules.Object);
        }

        // ───────────── GetAll Tests ─────────────

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts_WhenNoFilterApplied()
        {
            // Arrange
            var productsList = new List<ProductEntity>
            {
                new() { ProductId = 1, ProductName = "Product A", UnitPrice = 100m, UnitsInStock = 10, CategoryId = 1, SupplierId = 1 },
                new() { ProductId = 2, ProductName = "Product B", UnitPrice = 200m, UnitsInStock = 20, CategoryId = 1, SupplierId = 2 },
                new() { ProductId = 3, ProductName = "Product C", UnitPrice = 300m, UnitsInStock = 30, CategoryId = 2, SupplierId = 1 }
            };

            var mockDbSet = productsList.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<ProductEntity>().GetAll())
                .Returns(mockDbSet);

            var query = new GetProductsQuery { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _productService.GetAllAsync(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnFilteredProducts_WhenProductNameFilterApplied()
        {
            // Arrange
            var productsList = new List<ProductEntity>
            {
                new() { ProductId = 1, ProductName = "Apple Juice", UnitPrice = 100m, UnitsInStock = 10, CategoryId = 1, SupplierId = 1 },
                new() { ProductId = 2, ProductName = "Orange Juice", UnitPrice = 200m, UnitsInStock = 20, CategoryId = 1, SupplierId = 2 },
                new() { ProductId = 3, ProductName = "Apple Pie", UnitPrice = 300m, UnitsInStock = 30, CategoryId = 2, SupplierId = 1 }
            };

            var mockDbSet = productsList.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<ProductEntity>().GetAll())
                .Returns(mockDbSet);

            var query = new GetProductsQuery
            {
                ProductName = "Apple",
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _productService.GetAllAsync(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.All(p => p.ProductName.Contains("Apple")).Should().BeTrue();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnFilteredProducts_WhenPriceFilterApplied()
        {
            // Arrange
            var productsList = new List<ProductEntity>
            {
                new() { ProductId = 1, ProductName = "Cheap Item", UnitPrice = 50m, UnitsInStock = 10, CategoryId = 1, SupplierId = 1 },
                new() { ProductId = 2, ProductName = "Mid Item", UnitPrice = 150m, UnitsInStock = 20, CategoryId = 1, SupplierId = 2 },
                new() { ProductId = 3, ProductName = "Expensive Item", UnitPrice = 300m, UnitsInStock = 30, CategoryId = 2, SupplierId = 1 }
            };

            var mockDbSet = productsList.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<ProductEntity>().GetAll())
                .Returns(mockDbSet);

            var query = new GetProductsQuery
            {
                MinPrice = 100m,
                MaxPrice = 200m,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _productService.GetAllAsync(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result.First().UnitPrice.Should().Be(150m);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnFilteredProducts_WhenCategoryFilterApplied()
        {
            // Arrange
            var productsList = new List<ProductEntity>
            {
                new() { ProductId = 1, ProductName = "Product A", UnitPrice = 100m, UnitsInStock = 10, CategoryId = 1, SupplierId = 1 },
                new() { ProductId = 2, ProductName = "Product B", UnitPrice = 200m, UnitsInStock = 20, CategoryId = 2, SupplierId = 2 },
                new() { ProductId = 3, ProductName = "Product C", UnitPrice = 300m, UnitsInStock = 30, CategoryId = 1, SupplierId = 1 }
            };

            var mockDbSet = productsList.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<ProductEntity>().GetAll())
                .Returns(mockDbSet);

            var query = new GetProductsQuery
            {
                CategoryId = 1,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _productService.GetAllAsync(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.All(p => p.ProductId == 1 || p.ProductId == 3).Should().BeTrue();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnCorrectPage_WhenPaginationApplied()
        {
            // Arrange
            var productsList = Enumerable.Range(1, 20).Select(i => new ProductEntity
            {
                ProductId = i,
                ProductName = $"Product {i}",
                UnitPrice = 100m * i,
                UnitsInStock = (short)i,
                CategoryId = 1,
                SupplierId = 1
            }).ToList();

            var mockDbSet = productsList.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<ProductEntity>().GetAll())
                .Returns(mockDbSet);

            var query = new GetProductsQuery { PageNumber = 2, PageSize = 5 };

            // Act
            var result = await _productService.GetAllAsync(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(5);
        }

        // ───────────── Create Tests ─────────────

        [Fact]
        public async Task CreateAsync_ShouldCreateProduct_WhenValidRequest()
        {
            // Arrange
            var productsList = new List<ProductEntity>();
            var mockDbSet = productsList.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<ProductEntity>().GetAll())
                .Returns(mockDbSet);

            MockUnitOfWork
                .Setup(x => x.Repository<ProductEntity>().AddAsync(
                    It.IsAny<ProductEntity>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            MockUnitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var command = new CreateProductCommand
            {
                ProductName = "Test Product",
                CategoryId = 1,
                SupplierId = 1,
                UnitPrice = 99.99m,
                UnitsInStock = 100,
                QuantityPerUnit = "10 boxes x 10 units",
                UnitsOnOrder = 0,
                ReorderLevel = 10,
                Discontinued = false
            };

            // Act
            var result = await _productService.CreateAsync(command, CancellationToken.None);

            // Assert (STATE)
            result.Should().NotBeNull();
            result.ProductName.Should().Be("Test Product");
            result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

            // Assert (BEHAVIOR)
            MockUnitOfWork.Verify(x => x.Repository<ProductEntity>().AddAsync(
                It.Is<ProductEntity>(p =>
                    p.ProductName == "Test Product" &&
                    p.UnitPrice == 99.99m &&
                    p.CategoryId == 1
                ),
                It.IsAny<CancellationToken>()),
                Times.Once);

            MockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenProductAlreadyExists()
        {
            // Arrange
            _mockBusinessRules
                .Setup(x => x.ProductNameMustBeUniqueAsync(
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Existing Product"));

            var command = new CreateProductCommand
            {
                ProductName = "Existing Product",
                CategoryId = 1,
                SupplierId = 1,
                UnitPrice = 99.99m
            };

            // Act
            var act = async () => await _productService.CreateAsync(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*Existing Product*");
        }

        // ───────────── Update Tests ─────────────
        [Fact]
        public async Task UpdateAsync_ShouldUpdateProduct_WhenValidRequest()
        {
            // Arrange
            var product = new ProductEntity
            {
                ProductId = 1,
                ProductName = "Old Product",
                UnitPrice = 100m,
                UnitsInStock = 10,
                CategoryId = 1,
                SupplierId = 1
            };

            var productsList = new List<ProductEntity> { product };
            var mockDbSet = productsList.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<ProductEntity>().GetAll())
                .Returns(mockDbSet);

            _mockBusinessRules
                .Setup(x => x.ProductNameMustBeUniqueForUpdateAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            MockUnitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var command = new UpdateProductCommand
            {
                ProductId = 1,
                ProductName = "Updated Product",
                UnitPrice = 150m,
                UnitsInStock = 20,
                CategoryId = 2,
                SupplierId = 2,
                QuantityPerUnit = "5 boxes x 20 units",
                UnitsOnOrder = 5,
                ReorderLevel = 15,
                Discontinued = false
            };

            // Act
            var result = await _productService.UpdateAsync(command, CancellationToken.None);

            // Assert (STATE)
            result.Should().NotBeNull();
            result.ProductId.Should().Be(1);
            result.ProductName.Should().Be("Updated Product");

            
            _mockBusinessRules.Verify(x =>
                x.ProductNameMustBeUniqueForUpdateAsync(
                    command.ProductId,
                    command.ProductName,
                    It.IsAny<CancellationToken>()),
                Times.Once);

           
            MockUnitOfWork.Verify(x =>
                x.Repository<ProductEntity>().Update(
                    It.Is<ProductEntity>(p =>
                        p.ProductId == 1 &&
                        p.ProductName == "Updated Product" &&
                        p.UnitPrice == 150m &&
                        p.CategoryId == 2
                    )),
                Times.Once);

            
            MockUnitOfWork.Verify(x =>
                x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            var productsList = new List<ProductEntity>();
            var mockDbSet = productsList.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<ProductEntity>().GetAll())
                .Returns(mockDbSet);

            var command = new UpdateProductCommand
            {
                ProductId = 999,
                ProductName = "Non-existent Product",
                UnitPrice = 100m
            };

            // Act
            var act = async () => await _productService.UpdateAsync(command, CancellationToken.None);

            // Assert 
            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("*999*");

            
            _mockBusinessRules.Verify(x =>
                x.ProductNameMustBeUniqueForUpdateAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            
            MockUnitOfWork.Verify(x =>
                x.Repository<ProductEntity>().Update(It.IsAny<ProductEntity>()),
                Times.Never);

           
            MockUnitOfWork.Verify(x =>
                x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenProductNameAlreadyExists()
        {
            // Arrange
            var product = new ProductEntity
            {
                ProductId = 1,
                ProductName = "Old Product",
                UnitPrice = 100m
            };

            var productsList = new List<ProductEntity> { product };
            var mockDbSet = productsList.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<ProductEntity>().GetAll())
                .Returns(mockDbSet);

            _mockBusinessRules
                .Setup(x => x.ProductNameMustBeUniqueForUpdateAsync(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Duplicate product"));

            var command = new UpdateProductCommand
            {
                ProductId = 1,
                ProductName = "Existing Product"
            };

            // Act
            var act = async () => await _productService.UpdateAsync(command, CancellationToken.None);

            // Assert 
            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("*Duplicate product*");

            
            _mockBusinessRules.Verify(x =>
                x.ProductNameMustBeUniqueForUpdateAsync(
                    command.ProductId,
                    command.ProductName,
                    It.IsAny<CancellationToken>()),
                Times.Once);

          
            MockUnitOfWork.Verify(x =>
                x.Repository<ProductEntity>().Update(It.IsAny<ProductEntity>()),
                Times.Never);

           
            MockUnitOfWork.Verify(x =>
                x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        // ───────────── Delete Tests ─────────────

        [Fact]
        public async Task DeleteAsync_ShouldDeleteProduct_WhenProductExists()
        {
            // Arrange
            var productsList = new List<ProductEntity>
            {
                new() { ProductId = 1, ProductName = "Test Product", UnitPrice = 100m, UnitsInStock = 10, CategoryId = 1, SupplierId = 1 }
            };

            var mockDbSet = productsList.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<ProductEntity>().GetAll())
                .Returns(mockDbSet);

            MockUnitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _productService.DeleteAsync(1, CancellationToken.None);

            // Assert (STATE)
            result.Should().BeTrue();

            // Assert (BEHAVIOR)
            MockUnitOfWork.Verify(x => x.Repository<ProductEntity>().Delete(
                It.Is<ProductEntity>(p => p.ProductId == 1)), Times.Once);

            MockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            var productsList = new List<ProductEntity>();
            var mockDbSet = productsList.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<ProductEntity>().GetAll())
                .Returns(mockDbSet);

            // Act
            var act = async () => await _productService.DeleteAsync(999, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*999*");
        }
    }
}
