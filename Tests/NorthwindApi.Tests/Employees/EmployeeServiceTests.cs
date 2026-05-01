using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using NorthwindApi.Application.Features.Employees.Commands.CreateEmployee;
using NorthwindApi.Application.Features.Employees.Commands.DeleteEmployee;
using NorthwindApi.Application.Features.Employees.Commands.UpdateEmployee;
using NorthwindApi.Application.Features.Employees.Queries.GetEmployees;
using NorthwindApi.Application.Interfaces.BusinessRules;
using NorthwindApi.Domain.Entities;
using NorthwindApi.Persistence.Services.EntityServices;
using NorthwindApi.Tests.Common;

namespace NorthwindApi.Tests.Employees;

public class EmployeeServiceTests : TestBase
{
    private readonly Mock<IEmployeeBusinessRules> _mockBusinessRules;
    private readonly EmployeeService _employeeService;

    public EmployeeServiceTests()
    {
        _mockBusinessRules = new Mock<IEmployeeBusinessRules>();
        _employeeService = new EmployeeService(MockUnitOfWork.Object, _mockBusinessRules.Object);
    }

    // ------------- GetEmployees Tests -------------

    [Fact]
    public async Task GetEmployeesAsync_ShouldReturnAllEmployees_WhenNoFilterApplied()
    {
        var employees = new List<Employee>
        {
            new() { EmployeeId = 1, FirstName = "Nancy", LastName = "Davolio", Country = "USA" },
            new() { EmployeeId = 2, FirstName = "Andrew", LastName = "Fuller", Country = "USA" },
            new() { EmployeeId = 3, FirstName = "Janet", LastName = "Leverling", Country = "USA" }
        };

        var mockDbSet = employees.AsQueryable().BuildMock();
        MockUnitOfWork.Setup(x => x.Repository<Employee>().GetAll()).Returns(mockDbSet);

        var query = new GetEmployeesQuery { PageNumber = 1, PageSize = 10 };

        var result = await _employeeService.GetEmployeesAsync(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetEmployeesAsync_ShouldReturnFilteredEmployees_WhenFirstNameFilterApplied()
    {
        var employees = new List<Employee>
        {
            new() { EmployeeId = 1, FirstName = "Nancy", LastName = "Davolio" },
            new() { EmployeeId = 2, FirstName = "Andrew", LastName = "Fuller" },
            new() { EmployeeId = 3, FirstName = "Anne", LastName = "Dodsworth" }
        };

        var mockDbSet = employees.AsQueryable().BuildMock();
        MockUnitOfWork.Setup(x => x.Repository<Employee>().GetAll()).Returns(mockDbSet);

        var query = new GetEmployeesQuery { FirstName = "An", PageNumber = 1, PageSize = 10 };

        var result = await _employeeService.GetEmployeesAsync(query, CancellationToken.None);

        result.Should().HaveCount(2);
        result.All(e => e.FullName.Contains("An")).Should().BeTrue();
    }

    [Fact]
    public async Task GetEmployeesAsync_ShouldReturnFilteredEmployees_WhenCountryFilterApplied()
    {
        var employees = new List<Employee>
        {
            new() { EmployeeId = 1, FirstName = "Nancy", Country = "USA" },
            new() { EmployeeId = 2, FirstName = "Andrew", Country = "USA" },
            new() { EmployeeId = 3, FirstName = "Anne", Country = "UK" }
        };

        var mockDbSet = employees.AsQueryable().BuildMock();
        MockUnitOfWork.Setup(x => x.Repository<Employee>().GetAll()).Returns(mockDbSet);

        var query = new GetEmployeesQuery { Country = "UK", PageNumber = 1, PageSize = 10 };

        var result = await _employeeService.GetEmployeesAsync(query, CancellationToken.None);

        result.Should().HaveCount(1);
        result.First().Country.Should().Be("UK");
    }

    [Fact]
    public async Task GetEmployeesAsync_ShouldReturnFilteredEmployees_WhenTitleFilterApplied()
    {
        var employees = new List<Employee>
        {
            new() { EmployeeId = 1, Title = "Sales Representative" },
            new() { EmployeeId = 2, Title = "Vice President" },
            new() { EmployeeId = 3, Title = "Sales Manager" }
        };

        var mockDbSet = employees.AsQueryable().BuildMock();
        MockUnitOfWork.Setup(x => x.Repository<Employee>().GetAll()).Returns(mockDbSet);

        var query = new GetEmployeesQuery { Title = "Sales", PageNumber = 1, PageSize = 10 };

        var result = await _employeeService.GetEmployeesAsync(query, CancellationToken.None);

        result.Should().HaveCount(2);
        result.All(e => e.Title!.Contains("Sales")).Should().BeTrue();
    }

    // ------------- Create Tests -------------

    [Fact]
    public async Task CreateEmployeeAsync_ShouldCreateEmployee_WhenValidRequest()
    {
        _mockBusinessRules
            .Setup(x => x.ReportsToEmployeeMustExistAsync(It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockUnitOfWork
            .Setup(x => x.Repository<Employee>().AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockUnitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new CreateEmployeeCommand
        {
            FirstName = "John",
            LastName = "Doe",
            ReportsTo = 2
        };

        var result = await _employeeService.CreateEmployeeAsync(command, CancellationToken.None);

        result.FullName.Should().Be("John Doe");

        _mockBusinessRules.Verify(x =>
            x.ReportsToEmployeeMustExistAsync(2, It.IsAny<CancellationToken>()),
            Times.Once);

        MockUnitOfWork.Verify(x =>
            x.Repository<Employee>().AddAsync(
                It.Is<Employee>(e => e.FirstName == "John" && e.LastName == "Doe"),
                It.IsAny<CancellationToken>()),
            Times.Once);

        MockUnitOfWork.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateEmployeeAsync_ShouldThrowException_WhenReportsToNotFound()
    {
        _mockBusinessRules
            .Setup(x => x.ReportsToEmployeeMustExistAsync(999, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Manager not found"));

        var command = new CreateEmployeeCommand
        {
            FirstName = "John",
            LastName = "Doe",
            ReportsTo = 999
        };

        var act = async () => await _employeeService.CreateEmployeeAsync(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();

        
        _mockBusinessRules.Verify(x =>
            x.ReportsToEmployeeMustExistAsync(999, It.IsAny<CancellationToken>()),
            Times.Once);

        MockUnitOfWork.Verify(x =>
            x.Repository<Employee>().AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()),
            Times.Never);

        MockUnitOfWork.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    // ------------- Update Tests -------------

    [Fact]
    public async Task UpdateEmployeeAsync_ShouldUpdateEmployee_WhenValidRequest()
    {
        var employees = new List<Employee>
        {
            new() { EmployeeId = 1, FirstName = "Old", LastName = "Name" }
        };

        var mockDbSet = employees.AsQueryable().BuildMock();
        MockUnitOfWork.Setup(x => x.Repository<Employee>().GetAll()).Returns(mockDbSet);

        _mockBusinessRules.Setup(x => x.EmployeeMustExistAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockBusinessRules.Setup(x => x.ReportsToEmployeeMustExistAsync(It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new UpdateEmployeeCommand
        {
            EmployeeId = 1,
            FirstName = "Updated",
            LastName = "Employee"
        };

        var result = await _employeeService.UpdateEmployeeAsync(command, CancellationToken.None);

        result.FullName.Should().Be("Updated Employee");

        employees.First().FirstName.Should().Be("Updated");

        _mockBusinessRules.Verify(x =>
            x.EmployeeMustExistAsync(1, It.IsAny<CancellationToken>()),
            Times.Once);

        MockUnitOfWork.Verify(x =>
            x.Repository<Employee>().Update(It.IsAny<Employee>()),
            Times.Once);

        MockUnitOfWork.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ------------- Delete Tests -------------

    [Fact]
    public async Task DeleteEmployeeAsync_ShouldDeleteEmployee_WhenNoOrders()
    {
        var employees = new List<Employee>
        {
            new() { EmployeeId = 1 }
        };

        var mockDbSet = employees.AsQueryable().BuildMock();
        MockUnitOfWork.Setup(x => x.Repository<Employee>().GetAll()).Returns(mockDbSet);

        _mockBusinessRules.Setup(x => x.EmployeeMustExistAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _mockBusinessRules.Setup(x => x.EmployeeHasNoOrdersAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        MockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = new DeleteEmployeeCommand { EmployeeId = 1 };

        await _employeeService.DeleteEmployeeAsync(command, CancellationToken.None);

        _mockBusinessRules.Verify(x =>
            x.EmployeeMustExistAsync(1, It.IsAny<CancellationToken>()),
            Times.Once);

        _mockBusinessRules.Verify(x =>
            x.EmployeeHasNoOrdersAsync(1, It.IsAny<CancellationToken>()),
            Times.Once);

        MockUnitOfWork.Verify(x =>
            x.Repository<Employee>().Delete(It.Is<Employee>(e => e.EmployeeId == 1)),
            Times.Once);

        MockUnitOfWork.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}