using AutoMapper;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using NorthwindApi.Application.Features.Customers.Commands.CreateCustomer;
using NorthwindApi.Application.Features.Customers.Commands.UpdateCustomer;
using NorthwindApi.Application.Features.Customers.Queries.GetCustomers;
using NorthwindApi.Application.Interfaces.BusinessRules;
using NorthwindApi.Domain.Entities;
using NorthwindApi.Persistence.Services.EntityServices;
using NorthwindApi.Tests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Tests.Customers
{
    public class CustomerServiceTests : TestBase
    {
        private readonly Mock<ICustomerBusinessRules> _mockBusinessRules;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CustomerService _customerService;

        public CustomerServiceTests()
        {
            _mockBusinessRules = new Mock<ICustomerBusinessRules>();
            _mockMapper = new Mock<IMapper>();
            _customerService = new CustomerService(
                MockUnitOfWork.Object,
                _mockBusinessRules.Object,
                _mockMapper.Object);
        }

        // ───────────── GetAll Tests ─────────────

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCustomers_WhenNoFilterApplied()
        {
            // Arrange
            var customers = new List<Customer>
        {
            new() { CustomerId = "ALFKI", CompanyName = "Alfreds Futterkiste", Country = "Germany" },
            new() { CustomerId = "ANATR", CompanyName = "Ana Trujillo", Country = "Mexico" },
            new() { CustomerId = "ANTON", CompanyName = "Antonio Moreno", Country = "Mexico" }
        };

            var mockDbSet = customers.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<Customer>().GetAll())
                .Returns(mockDbSet);

            _mockMapper
                .Setup(x => x.Map<List<GetCustomersQueryResponse>>(It.IsAny<List<Customer>>()))
                .Returns(customers.Select(c => new GetCustomersQueryResponse
                {
                    CustomerId = c.CustomerId,
                    CompanyName = c.CompanyName
                }).ToList());

            var query = new GetCustomersQuery { PageNumber = 1, PageSize = 10 };

            // Act
            var result = await _customerService.GetAllAsync(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnFilteredCustomers_WhenCountryFilterApplied()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new() { CustomerId = "ALFKI", CompanyName = "A", Country = "Germany" },
                new() { CustomerId = "ANATR", CompanyName = "B", Country = "Mexico" },
                new() { CustomerId = "ANTON", CompanyName = "C", Country = "Mexico" }
            };

            var mockDbSet = customers.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<Customer>().GetAll())
                .Returns(mockDbSet);

            _mockMapper
                .Setup(x => x.Map<List<GetCustomersQueryResponse>>(It.IsAny<List<Customer>>() ))
                .Returns((List<Customer> src) => src.Select(c => new GetCustomersQueryResponse
                {
                    CustomerId = c.CustomerId,
                    CompanyName = c.CompanyName,
                    Country = c.Country
                }).ToList());

            var query = new GetCustomersQuery
            {
                Country = "Germany",
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _customerService.GetAllAsync(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result.First().Country.Should().Be("Germany");
        }


        // ───────────── Create Tests ─────────────

        [Fact]
        public async Task CreateAsync_ShouldCreateCustomer_WhenValidRequest()
        {
            // Arrange

            var customers = new List<Customer>();
            var mockDbSet = customers.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<Customer>().GetAll())
                .Returns(mockDbSet);

            var command = new CreateCustomerCommand
            {
                CustomerId = "TESTX",
                CompanyName = "Test Company"
            };

            var expectedCustomer = new Customer
            {
                CustomerId = command.CustomerId,
                CompanyName = command.CompanyName
            };

            _mockMapper
                .Setup(x => x.Map<Customer>(It.Is<CreateCustomerCommand>(cmd =>
                    cmd.CustomerId == command.CustomerId &&
                    cmd.CompanyName == command.CompanyName)))
                .Returns(expectedCustomer);

            MockUnitOfWork
                .Setup(x => x.Repository<Customer>()
                    .AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            MockUnitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _customerService.CreateAsync(command, CancellationToken.None);

            // Assert (STATE)
            result.Should().NotBeNull();
            result.CustomerId.Should().Be(command.CustomerId);
            result.CompanyName.Should().Be(command.CompanyName);

            // Assert (BEHAVIOR)

            // 1. Mapper doğru çağrıldı mı?
            _mockMapper.Verify(x => x.Map<Customer>(
                It.Is<CreateCustomerCommand>(cmd =>
                    cmd.CustomerId == command.CustomerId &&
                    cmd.CompanyName == command.CompanyName
                )),
                Times.Once);

            // 2. Repository’ye doğru entity gönderildi mi?
            MockUnitOfWork.Verify(x => x.Repository<Customer>().AddAsync(
                It.Is<Customer>(c =>
                    c.CustomerId == command.CustomerId &&
                    c.CompanyName == command.CompanyName
                ),
                It.IsAny<CancellationToken>()),
                Times.Once);

            // 3. SaveChanges çağrıldı mı?
            MockUnitOfWork.Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowException_WhenCustomerAlreadyExists()
        {
            // Arrange
            var customers = new List<Customer>
        {
            new() { CustomerId = "TESTX", CompanyName = "Existing Company" }
        };

            var mockDbSet = customers.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<Customer>().GetAll())
                .Returns(mockDbSet);

            var command = new CreateCustomerCommand
            {
                CustomerId = "TESTX",
                CompanyName = "Test Company"
            };

            // Act
            var act = async () => await _customerService.CreateAsync(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*TESTX*");
        }

     
        // ───────────── Update Tests ─────────────


        [Fact]
        public async Task UpdateAsync_ShouldUpdateCustomer_WhenValidRequest()
        {
            // Arrange
            var customers = new List<Customer>
             {
                new() { CustomerId = "TESTX", CompanyName = "Old Company" }
             };

            var mockDbSet = customers.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<Customer>().GetAll())
                .Returns(mockDbSet);

            MockUnitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            _mockMapper
                .Setup(x => x.Map(It.IsAny<UpdateCustomerCommand>(), It.IsAny<Customer>()))
                .Callback<UpdateCustomerCommand, Customer>((cmd, entity) =>
                {
                    entity.CompanyName = cmd.CompanyName;
                    entity.ContactName = cmd.ContactName;
                });

            var command = new UpdateCustomerCommand
            {
                CustomerId = "TESTX",
                CompanyName = "Updated Company",
                ContactName = "Updated Contact"
            };

            // Act
            var result = await _customerService.UpdateAsync(command, CancellationToken.None);

            // Assert
            result.CompanyName.Should().Be("Updated Company");

            MockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            _mockMapper.Verify(x => x.Map(command, It.IsAny<Customer>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenCustomerNotFound()
        {
            // Arrange
            var customers = new List<Customer>();
            var mockDbSet = customers.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<Customer>().GetAll())
                .Returns(mockDbSet);

            var command = new UpdateCustomerCommand
            {
                CustomerId = "XXXXX",
                CompanyName = "Test Company"
            };

            // Act
            var act = async () => await _customerService.UpdateAsync(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*XXXXX*");
        }

        // ───────────── Delete Tests ─────────────

        [Fact]
        public async Task DeleteAsync_ShouldDeleteCustomer_WhenCustomerExists()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new() { CustomerId = "TESTX", CompanyName = "Test Company" }
            };

            var mockDbSet = customers.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<Customer>().GetAll())
                .Returns(mockDbSet);

            MockUnitOfWork
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _customerService.DeleteAsync("TESTX", CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            MockUnitOfWork.Verify(x => x.Repository<Customer>().Delete(
                It.Is<Customer>(c => c.CustomerId == "TESTX")), Times.Once);

            MockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenCustomerNotFound()
        {
            // Arrange
            var customers = new List<Customer>();
            var mockDbSet = customers.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<Customer>().GetAll())
                .Returns(mockDbSet);

            // Act
            var act = async () => await _customerService.DeleteAsync("XXXXX", CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("*XXXXX*");
        }

        // ───────────── Pagination Tests ─────────────

        [Fact]
        public async Task GetAllAsync_ShouldReturnCorrectPage_WhenPaginationApplied()
        {
            // Arrange
            var customers = Enumerable.Range(1, 20).Select(i => new Customer
            {
                CustomerId = $"CUS{i:D2}",
                CompanyName = $"Company {i}",
                Country = "Turkey"
            }).ToList();

            var mockDbSet = customers.AsQueryable().BuildMock();

            MockUnitOfWork
                .Setup(x => x.Repository<Customer>().GetAll())
                .Returns(mockDbSet);

            _mockMapper
                .Setup(x => x.Map<List<GetCustomersQueryResponse>>(It.IsAny<List<Customer>>()))
                .Returns((List<Customer> src) => src.Select(c => new GetCustomersQueryResponse
                {
                    CustomerId = c.CustomerId,
                    CompanyName = c.CompanyName
                }).ToList());

            var query = new GetCustomersQuery { PageNumber = 2, PageSize = 5 };

            // Act
            var result = await _customerService.GetAllAsync(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(5);
        }
    }
}
