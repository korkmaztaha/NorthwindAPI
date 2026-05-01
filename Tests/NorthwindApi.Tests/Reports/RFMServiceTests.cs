using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using NorthwindApi.Application.Common.BusinessRules;
using NorthwindApi.Application.Features.Reports.GetCustomerRFM;
using NorthwindApi.Domain.Entities;
using NorthwindApi.Persistence.Services.EntityServices;
using NorthwindApi.Tests.Common;

namespace NorthwindApi.Tests.Reports;

public class RFMTests : TestBase
{
    private readonly ReportService _reportService;

    public RFMTests()
    {
        _reportService = new ReportService(MockUnitOfWork.Object);
    }

    // ───────────── RFMSegmentCalculator Tests ─────────────

    [Theory]
    [InlineData(5, 5, 5, "Champions")]
    [InlineData(4, 4, 4, "Champions")]
    [InlineData(4, 5, 4, "Champions")]
    public void DetermineSegment_ShouldReturnChampions_WhenAllScoresHigh(int r, int f, int m, string expected)
    {
        var result = RFMSegmentCalculator.DetermineSegment(r, f, m);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(3, 3, 2, "Loyal")]
    [InlineData(4, 3, 1, "Loyal")]
    [InlineData(3, 4, 5, "Loyal")]
    public void DetermineSegment_ShouldReturnLoyal_WhenRecencyAndFrequencyAreMediumHigh(int r, int f, int m, string expected)
    {
        var result = RFMSegmentCalculator.DetermineSegment(r, f, m);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(2, 3, 2, "AtRisk")]
    [InlineData(1, 4, 3, "AtRisk")]
    [InlineData(2, 5, 5, "AtRisk")]
    public void DetermineSegment_ShouldReturnAtRisk_WhenRecencyLowFrequencyHigh(int r, int f, int m, string expected)
    {
        var result = RFMSegmentCalculator.DetermineSegment(r, f, m);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, 1, 1, "Lost")]
    [InlineData(1, 2, 3, "Lost")]
    [InlineData(1, 2, 5, "Lost")]
    public void DetermineSegment_ShouldReturnLost_WhenRecencyOneAndFrequencyLow(int r, int f, int m, string expected)
    {
        var result = RFMSegmentCalculator.DetermineSegment(r, f, m);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(4, 1, 1, "NewCustomers")]
    [InlineData(5, 1, 3, "NewCustomers")]
    [InlineData(4, 1, 5, "NewCustomers")]
    public void DetermineSegment_ShouldReturnNewCustomers_WhenRecencyHighFrequencyOne(int r, int f, int m, string expected)
    {
        var result = RFMSegmentCalculator.DetermineSegment(r, f, m);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(2, 2, 2, "Others")]
    [InlineData(3, 2, 3, "Others")]
    [InlineData(2, 1, 4, "Others")]
    public void DetermineSegment_ShouldReturnOthers_WhenNoSegmentMatches(int r, int f, int m, string expected)
    {
        var result = RFMSegmentCalculator.DetermineSegment(r, f, m);
        result.Should().Be(expected);
    }

    // ───────────── GetCustomerRFMAsync Tests ─────────────

    [Fact]
    public async Task GetCustomerRFMAsync_ShouldReturnResult_WhenCustomersExist()
    {
        // Arrange - En az 5 müşteri lazım percentile hesabı için
        var customers = Enumerable.Range(1, 10).Select(i => new Customer
        {
            CustomerId = $"CUS{i:D2}",
            CompanyName = $"Company {i}",
            Country = "USA",
            Orders = new List<Orders>
        {
            new()
            {
                OrderId = i,
                OrderDate = DateTime.UtcNow.AddDays(-i * 30),
                OrderDetails = new List<OrderDetails>
                {
                    new() { Quantity = (short)(i * 2), UnitPrice = i * 10m, Discount = 0 }
                }
            }
        }
        }).ToList();

        var mockDbSet = customers.AsQueryable().BuildMock();
        MockUnitOfWork.Setup(x => x.Repository<Customer>().GetAll()).Returns(mockDbSet);

        var query = new GetCustomerRFMQuery { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _reportService.GetCustomerRFMAsync(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        result.Items.Should().NotBeNull();
        result.Items.Should().HaveCount(10);

        result.Summary.Should().NotBeNull();
        result.Summary.TotalCustomers.Should().Be(10);

        (
            result.Summary.Champions +
            result.Summary.Loyal +
            result.Summary.AtRisk +
            result.Summary.Lost +
            result.Summary.NewCustomers +
            result.Summary.Others
        ).Should().Be(result.Summary.TotalCustomers);
    }

    [Fact]
    public async Task GetCustomerRFMAsync_ShouldFilterByCountry_WhenCountryProvided()
    {
        // Arrange
        var customers = Enumerable.Range(1, 10).Select(i => new Customer
        {
            CustomerId = $"CUS{i:D2}",
            CompanyName = $"Company {i}",
            Country = i <= 5 ? "USA" : "Germany",
            Orders = new List<Orders>
            {
                new()
                {
                    OrderId = i,
                    OrderDate = DateTime.UtcNow.AddDays(-i * 30),
                    OrderDetails = new List<OrderDetails>
                    {
                        new() { Quantity = (short)(i * 2), UnitPrice = i * 10m, Discount = 0 }
                    }
                }
            }
        }).ToList();

        var mockDbSet = customers.AsQueryable().BuildMock();
        MockUnitOfWork.Setup(x => x.Repository<Customer>().GetAll()).Returns(mockDbSet);

        var query = new GetCustomerRFMQuery { Country = "USA", PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _reportService.GetCustomerRFMAsync(query, CancellationToken.None);

        // Assert
        result.Summary.TotalCustomers.Should().Be(5);
        result.Items.All(x => x.Country == "USA").Should().BeTrue();
    }

    [Fact]
    public async Task GetCustomerRFMAsync_ShouldFilterBySegment_WhenSegmentProvided()
    {
        // Arrange
        var customers = Enumerable.Range(1, 10).Select(i => new Customer
        {
            CustomerId = $"CUS{i:D2}",
            CompanyName = $"Company {i}",
            Country = "USA",
            Orders = new List<Orders>
            {
                new()
                {
                    OrderId = i,
                    OrderDate = DateTime.UtcNow.AddDays(-i * 30),
                    OrderDetails = new List<OrderDetails>
                    {
                        new() { Quantity = (short)(i * 2), UnitPrice = i * 10m, Discount = 0 }
                    }
                }
            }
        }).ToList();

        var mockDbSet = customers.AsQueryable().BuildMock();
        MockUnitOfWork.Setup(x => x.Repository<Customer>().GetAll()).Returns(mockDbSet);

        var query = new GetCustomerRFMQuery { Segment = "Champions", PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _reportService.GetCustomerRFMAsync(query, CancellationToken.None);

        // Assert
        result.Items.All(x => x.Segment == "Champions").Should().BeTrue();
        result.Summary.TotalCustomers.Should().Be(10); // Summary tüm müşterileri gösterir
    }

    [Fact]
    public async Task GetCustomerRFMAsync_ShouldReturnCorrectRFMScoreRange()
    {
        // Arrange
        var customers = Enumerable.Range(1, 10).Select(i => new Customer
        {
            CustomerId = $"CUS{i:D2}",
            CompanyName = $"Company {i}",
            Country = "USA",
            Orders = new List<Orders>
            {
                new()
                {
                    OrderId = i,
                    OrderDate = DateTime.UtcNow.AddDays(-i * 30),
                    OrderDetails = new List<OrderDetails>
                    {
                        new() { Quantity = (short)(i * 2), UnitPrice = i * 10m, Discount = 0 }
                    }
                }
            }
        }).ToList();

        var mockDbSet = customers.AsQueryable().BuildMock();
        MockUnitOfWork.Setup(x => x.Repository<Customer>().GetAll()).Returns(mockDbSet);

        var query = new GetCustomerRFMQuery { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _reportService.GetCustomerRFMAsync(query, CancellationToken.None);

        // Assert - RFM skoru 3-15 arasında olmalı (her skor 1-5)
        result.Items.All(x => x.RFMScore >= 3 && x.RFMScore <= 15).Should().BeTrue();
        result.Items.All(x => x.RecencyScore >= 1 && x.RecencyScore <= 5).Should().BeTrue();
        result.Items.All(x => x.FrequencyScore >= 1 && x.FrequencyScore <= 5).Should().BeTrue();
        result.Items.All(x => x.MonetaryScore >= 1 && x.MonetaryScore <= 5).Should().BeTrue();
    }

    [Fact]
    public async Task GetCustomerRFMAsync_ShouldReturnItemsOrderedByRFMScore()
    {
        // Arrange
        var customers = Enumerable.Range(1, 10).Select(i => new Customer
        {
            CustomerId = $"CUS{i:D2}",
            CompanyName = $"Company {i}",
            Country = "USA",
            Orders = new List<Orders>
            {
                new()
                {
                    OrderId = i,
                    OrderDate = DateTime.UtcNow.AddDays(-i * 30),
                    OrderDetails = new List<OrderDetails>
                    {
                        new() { Quantity = (short)(i * 2), UnitPrice = i * 10m, Discount = 0 }
                    }
                }
            }
        }).ToList();

        var mockDbSet = customers.AsQueryable().BuildMock();
        MockUnitOfWork.Setup(x => x.Repository<Customer>().GetAll()).Returns(mockDbSet);

        var query = new GetCustomerRFMQuery { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _reportService.GetCustomerRFMAsync(query, CancellationToken.None);

        // Assert - RFM skoruna göre azalan sıralı olmalı
        result.Items.Should().BeInDescendingOrder(x => x.RFMScore);
    }
}