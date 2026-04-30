using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using NorthwindApi.Application.Features.Shippers.Commands.CreateShipper;
using NorthwindApi.Application.Features.Shippers.Commands.UpdateShipper;
using NorthwindApi.Application.Features.Shippers.Queries.GetShippers;
using NorthwindApi.Application.Interfaces.BusinessRules;
using NorthwindApi.Domain.Entities;
using ShipperEntity = NorthwindApi.Domain.Entities.Shippers;
using NorthwindApi.Persistence.Services.EntityServices;
using NorthwindApi.Tests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Tests.Shippers
{
    public class ShipperServiceTests : TestBase
    {
        private readonly Mock<IShipperBusinessRules> _mockBusinessRules;
        private readonly ShipperService _shipperService;

        public ShipperServiceTests()
        {
            _mockBusinessRules = new Mock<IShipperBusinessRules>();
            _shipperService = new ShipperService(MockUnitOfWork.Object, _mockBusinessRules.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllShippers_WhenNoFilterApplied()
        {
            // Arrange
            var shippers = new List<ShipperEntity>
            {
                new() { ShipperId = 1, CompanyName = "Alpha", Phone = "111" },
                new() { ShipperId = 2, CompanyName = "Beta", Phone = "222" },
                new() { ShipperId = 3, CompanyName = "Gamma", Phone = "333" }
            };

            var mockDbSet = shippers.AsQueryable().BuildMock();
            MockUnitOfWork.Setup(x => x.Repository<ShipperEntity>().GetAll()).Returns(mockDbSet);

            var query = new GetShippersQuery { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _shipperService.GetAllAsync(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnFilteredShippers_WhenCompanyNameFilterApplied()
        {
            // Arrange
            var shippers = new List<ShipperEntity>
            {
                new() { ShipperId = 1, CompanyName = "FastShip", Phone = "111" },
                new() { ShipperId = 2, CompanyName = "SlowShip", Phone = "222" },
                new() { ShipperId = 3, CompanyName = "FastDelivery", Phone = "333" }
            };

            var mockDbSet = shippers.AsQueryable().BuildMock();
            MockUnitOfWork.Setup(x => x.Repository<ShipperEntity>().GetAll()).Returns(mockDbSet);

            var query = new GetShippersQuery { CompanyName = "Fast", PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _shipperService.GetAllAsync(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.All(s => s.CompanyName.Contains("Fast")).Should().BeTrue();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateShipper_WhenValidRequest()
        {
            // Arrange
            var shippers = new List<ShipperEntity>();
            var mockDbSet = shippers.AsQueryable().BuildMock();
            MockUnitOfWork.Setup(x => x.Repository<ShipperEntity>().GetAll()).Returns(mockDbSet);

            _mockBusinessRules.Setup(x => x.ShipperCompanyNameMustBeUniqueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            MockUnitOfWork.Setup(x => x.Repository<ShipperEntity>().AddAsync(It.IsAny<ShipperEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            MockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var command = new CreateShipperCommand { CompanyName = "NewShip", Phone = "999" };

            // Act
            var result = await _shipperService.CreateAsync(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.CompanyName.Should().Be("NewShip");

            MockUnitOfWork.Verify(x => x.Repository<ShipperEntity>().AddAsync(It.IsAny<ShipperEntity>(), It.IsAny<CancellationToken>()), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenCompanyNameNotUnique()
        {
            // Arrange
            var shippers = new List<ShipperEntity>
            {
                new() { ShipperId = 1, CompanyName = "Existing", Phone = "111" }
            };
            var mockDbSet = shippers.AsQueryable().BuildMock();
            MockUnitOfWork.Setup(x => x.Repository<ShipperEntity>().GetAll()).Returns(mockDbSet);

            _mockBusinessRules.Setup(x => x.ShipperCompanyNameMustBeUniqueAsync("Existing", It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Existing name"));

            var command = new CreateShipperCommand { CompanyName = "Existing", Phone = "111" };

            // Act
            var act = async () => await _shipperService.CreateAsync(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateShipper_WhenValidRequest()
        {
            // Arrange
            var shippers = new List<ShipperEntity>
            {
                new() { ShipperId = 1, CompanyName = "OldName", Phone = "111" }
            };
            var mockDbSet = shippers.AsQueryable().BuildMock();
            MockUnitOfWork.Setup(x => x.Repository<ShipperEntity>().GetAll()).Returns(mockDbSet);

            _mockBusinessRules.Setup(x => x.ShipperMustExistAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            MockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var command = new UpdateShipperCommand { ShipperId = 1, CompanyName = "NewName", Phone = "222" };

            // Act
            var result = await _shipperService.UpdateAsync(command, CancellationToken.None);

            // Assert
            result.CompanyName.Should().Be("NewName");
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenShipperNotFound()
        {
            // Arrange
            var shippers = new List<ShipperEntity>();
            var mockDbSet = shippers.AsQueryable().BuildMock();
            MockUnitOfWork.Setup(x => x.Repository<ShipperEntity>().GetAll()).Returns(mockDbSet);

            _mockBusinessRules.Setup(x => x.ShipperMustExistAsync(999, It.IsAny<CancellationToken>())).ThrowsAsync(new KeyNotFoundException("not found"));

            var command = new UpdateShipperCommand { ShipperId = 999, CompanyName = "Name", Phone = "000" };

            // Act
            var act = async () => await _shipperService.UpdateAsync(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteShipper_WhenNoOrders()
        {
            // Arrange
            var shippers = new List<ShipperEntity>
            {
                new() { ShipperId = 1, CompanyName = "ToDelete", Phone = "111" }
            };
            var mockDbSet = shippers.AsQueryable().BuildMock();
            MockUnitOfWork.Setup(x => x.Repository<ShipperEntity>().GetAll()).Returns(mockDbSet);

            _mockBusinessRules.Setup(x => x.ShipperMustExistAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mockBusinessRules.Setup(x => x.ShipperHasNoOrdersAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            MockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _shipperService.DeleteAsync(1, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            MockUnitOfWork.Verify(x => x.Repository<ShipperEntity>().Delete(It.IsAny<ShipperEntity>()), Times.Once);
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenShipperHasOrders()
        {
            // Arrange
            var shippers = new List<ShipperEntity>
            {
                new() { ShipperId = 1, CompanyName = "HasOrders", Phone = "111", Orders = new List<Orders> { new Orders { OrderId = 1 } } }
            };
            var mockDbSet = shippers.AsQueryable().BuildMock();
            MockUnitOfWork.Setup(x => x.Repository<ShipperEntity>().GetAll()).Returns(mockDbSet);

            _mockBusinessRules.Setup(x => x.ShipperMustExistAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            _mockBusinessRules.Setup(x => x.ShipperHasNoOrdersAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(new InvalidOperationException("has orders"));

            // Act
            var act = async () => await _shipperService.DeleteAsync(1, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }
    }
}
