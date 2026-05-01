using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Common.BusinessRules;
using NorthwindApi.Application.Common.Helpers;
using NorthwindApi.Application.Features.Reports.GetCustomerRFM;
using NorthwindApi.Application.Features.Reports.GetEmployeePerformance;
using NorthwindApi.Application.Features.Reports.GetSalesByEmployee;
using NorthwindApi.Application.Features.Reports.GetSalesByProduct;
using NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByCategory;
using NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByPeriod;
using NorthwindApi.Application.Features.Reports.GetStockAnalysis;
using NorthwindApi.Application.Features.Reports.GetTopSellingProducts;
using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Application.Interfaces.Services;
using NorthwindApi.Domain.Entities;
using NorthwindApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Persistence.Services.EntityServices
{

    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<GetSalesByPeriodResponse>> GetSalesByPeriodAsync(
        GetSalesByPeriodQuery request,
        CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Orders>()
                .GetAll()
                .Where(o => o.OrderDate.HasValue);

            if (request.Year.HasValue)
                query = query.Where(o => o.OrderDate!.Value.Year == request.Year.Value);

            if (request.Month.HasValue)
                query = query.Where(o => o.OrderDate!.Value.Month == request.Month.Value);

            List<GetSalesByPeriodResponse> result;

            if (request.GroupBy == GroupBy.Year)
            {
                result = await query
                    .GroupBy(o => o.OrderDate!.Value.Year)
                    .Select(g => new GetSalesByPeriodResponse
                    {
                        Year = g.Key,
                        Month = null,
                        MonthName = null,
                        TotalOrders = g.Count(),
                        TotalSales = g.SelectMany(o => o.OrderDetails)
                            .Sum(od => (decimal)(od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount))),
                        TotalFreight = g.Sum(o => o.Freight ?? 0),
                        AverageOrderValue = g.SelectMany(o => o.OrderDetails)
                            .Average(od => (decimal)(od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount))),
                        TotalItemsSold = g.SelectMany(o => o.OrderDetails)
                            .Sum(od => (int)od.Quantity)
                    })
                    .OrderBy(x => x.Year)
                    .ToListAsync(cancellationToken);
            }
            else
            {
                result = await query
                    .GroupBy(o => new
                    {
                        Year = o.OrderDate!.Value.Year,
                        Month = o.OrderDate!.Value.Month
                    })
                    .Select(g => new GetSalesByPeriodResponse
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        MonthName = null,
                        TotalOrders = g.Count(),
                        TotalSales = g.SelectMany(o => o.OrderDetails)
                            .Sum(od => (decimal)(od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount))),
                        TotalFreight = g.Sum(o => o.Freight ?? 0),
                        AverageOrderValue = g.SelectMany(o => o.OrderDetails)
                            .Average(od => (decimal)(od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount))),
                        TotalItemsSold = g.SelectMany(o => o.OrderDetails)
                            .Sum(od => (int)od.Quantity)
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToListAsync(cancellationToken);
            }


            foreach (var item in result.Where(x => x.Month.HasValue))
                item.MonthName = DateHelper.GetMonthName(item.Year, item.Month!.Value);

            return result;
        }

        public async Task<List<GetSalesByCategoryResponse>> GetSalesByCategoryAsync(
        GetSalesByCategoryQuery request,
        CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Categories>()
                .GetAll()
                .Select(c => new GetSalesByCategoryResponse
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,


                    TotalOrders = c.Products
                        .SelectMany(p => p.OrderDetails)
                        .Select(od => od.OrderId)
                        .Distinct()
                        .Count(),

                    TotalProductsSold = c.Products
                        .SelectMany(p => p.OrderDetails)
                        .Sum(od => (int)od.Quantity),

                    TotalSales = c.Products
                        .SelectMany(p => p.OrderDetails)
                        .Sum(od => (decimal)(od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount))),

                    AverageOrderValue = c.Products
                        .SelectMany(p => p.OrderDetails)
                        .Any()
                            ? c.Products
                                .SelectMany(p => p.OrderDetails)
                                .Average(od => (decimal)(od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount)))
                            : 0,


                    TopSellingProduct = c.Products
                        .OrderByDescending(p => p.OrderDetails.Sum(od => od.Quantity))
                        .Select(p => p.ProductName)
                        .FirstOrDefault() ?? "N/A"
                });


            if (request.Year.HasValue || request.Month.HasValue)
            {

                var orderQuery = _unitOfWork.Repository<Orders>()
                    .GetAll()
                    .Where(o => o.OrderDate.HasValue);

                if (request.Year.HasValue)
                    orderQuery = orderQuery.Where(o => o.OrderDate!.Value.Year == request.Year.Value);

                if (request.Month.HasValue)
                    orderQuery = orderQuery.Where(o => o.OrderDate!.Value.Month == request.Month.Value);

                var result = await orderQuery
                    .SelectMany(o => o.OrderDetails)
                    .GroupBy(od => new
                    {
                        od.Product!.CategoryId,
                        CategoryName = od.Product.Category!.CategoryName
                    })
                    .Select(g => new GetSalesByCategoryResponse
                    {
                        CategoryId = g.Key.CategoryId ?? 0,
                        CategoryName = g.Key.CategoryName,
                        TotalOrders = g.Select(od => od.OrderId).Distinct().Count(),
                        TotalProductsSold = g.Sum(od => (int)od.Quantity),
                        TotalSales = g.Sum(od => (decimal)(od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount))),
                        AverageOrderValue = g.Any() ? g.Average(od => od.Quantity * od.UnitPrice * (1 - (decimal)od.Discount)) : 0,
                        TopSellingProduct = g.GroupBy(od => od.Product!.ProductName)
                            .OrderByDescending(pg => pg.Sum(od => od.Quantity))
                            .Select(pg => pg.Key)
                            .FirstOrDefault() ?? "N/A"
                    })
                    .OrderByDescending(x => x.TotalSales)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                return result;
            }

            return await query
                .OrderByDescending(x => x.TotalSales)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<GetStockAnalysisResponse> GetStockAnalysisAsync(
        GetStockAnalysisQuery request,
        CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Products>().GetAll();
            List<StockAnalysisItemResponse> items;

            switch (request.AnalysisType)
            {
                case StockAnalysisType.Critical:
                    items = await query
                        .Where(p => p.UnitsInStock < p.ReorderLevel && !p.Discontinued)
                        .OrderBy(p => p.UnitsInStock)
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .Select(p => new StockAnalysisItemResponse
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            CategoryName = p.Category != null ? p.Category.CategoryName : null,
                            SupplierName = p.Supplier != null ? p.Supplier.CompanyName : null,
                            UnitsInStock = p.UnitsInStock,
                            UnitsOnOrder = p.UnitsOnOrder,
                            ReorderLevel = p.ReorderLevel,
                            UnitPrice = p.UnitPrice,
                            Discontinued = p.Discontinued,
                            StockDeficit = (short?)(p.ReorderLevel - p.UnitsInStock)
                        })
                        .ToListAsync(cancellationToken);
                    break;

                case StockAnalysisType.Excess:
                    var cutoffDate = DateTime.UtcNow.AddDays(-(request.DaysSinceLastSale ?? 180));
                    items = await query
                        .Where(p => !p.Discontinued && p.UnitsInStock > 0)
                        .Where(p => !p.OrderDetails.Any(od =>
                            od.Order!.OrderDate.HasValue &&
                            od.Order.OrderDate.Value >= cutoffDate))
                        .OrderByDescending(p => p.UnitsInStock)
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .Select(p => new StockAnalysisItemResponse
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            CategoryName = p.Category != null ? p.Category.CategoryName : null,
                            SupplierName = p.Supplier != null ? p.Supplier.CompanyName : null,
                            UnitsInStock = p.UnitsInStock,
                            UnitsOnOrder = p.UnitsOnOrder,
                            ReorderLevel = p.ReorderLevel,
                            UnitPrice = p.UnitPrice,
                            Discontinued = p.Discontinued,
                            LastSaleDate = p.OrderDetails
                                .Where(od => od.Order!.OrderDate.HasValue)
                                .OrderByDescending(od => od.Order!.OrderDate)
                                .Select(od => od.Order!.OrderDate)
                                .FirstOrDefault(),
                            DaysSinceLastSale = p.OrderDetails.Any()
                                ? (int)(DateTime.UtcNow - p.OrderDetails
                                    .Where(od => od.Order!.OrderDate.HasValue)
                                    .OrderByDescending(od => od.Order!.OrderDate)
                                    .Select(od => od.Order!.OrderDate!.Value)
                                    .FirstOrDefault()).TotalDays
                                : null
                        })
                        .ToListAsync(cancellationToken);
                    break;

                case StockAnalysisType.Turnover:
                    var last12Months = DateTime.UtcNow.AddMonths(-12);
                    items = await query
                        .Where(p => !p.Discontinued && p.UnitsInStock > 0)
                        .OrderByDescending(p =>
                            p.OrderDetails
                                .Where(od => od.Order!.OrderDate >= last12Months)
                                .Sum(od => (int?)od.Quantity) ?? 0)
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .Select(p => new StockAnalysisItemResponse
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            CategoryName = p.Category != null ? p.Category.CategoryName : null,
                            SupplierName = p.Supplier != null ? p.Supplier.CompanyName : null,
                            UnitsInStock = p.UnitsInStock,
                            UnitsOnOrder = p.UnitsOnOrder,
                            ReorderLevel = p.ReorderLevel,
                            UnitPrice = p.UnitPrice,
                            Discontinued = p.Discontinued,
                            TotalSoldLast12Months = p.OrderDetails
                                .Where(od => od.Order!.OrderDate >= last12Months)
                                .Sum(od => (int?)od.Quantity) ?? 0,
                            TurnoverRate = p.UnitsInStock > 0
                                ? (decimal)(p.OrderDetails
                                    .Where(od => od.Order!.OrderDate >= last12Months)
                                    .Sum(od => (int?)od.Quantity) ?? 0) / p.UnitsInStock
                                : 0
                        })
                        .ToListAsync(cancellationToken);
                    break;

                case StockAnalysisType.Discontinued:
                    items = await query
                        .Where(p => p.Discontinued)
                        .OrderByDescending(p => p.OrderDetails.Sum(od => (int?)od.Quantity) ?? 0)
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .Select(p => new StockAnalysisItemResponse
                        {
                            ProductId = p.ProductId,
                            ProductName = p.ProductName,
                            CategoryName = p.Category != null ? p.Category.CategoryName : null,
                            SupplierName = p.Supplier != null ? p.Supplier.CompanyName : null,
                            UnitsInStock = p.UnitsInStock,
                            UnitPrice = p.UnitPrice,
                            Discontinued = p.Discontinued,
                            TotalOrders = p.OrderDetails.Select(od => od.OrderId).Distinct().Count(),
                            TotalRevenue = p.OrderDetails
                                .Sum(od => (decimal?)(od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount))) ?? 0
                        })
                        .ToListAsync(cancellationToken);
                    break;

                default:
                    items = new List<StockAnalysisItemResponse>();
                    break;
            }

            return new GetStockAnalysisResponse
            {
                Items = items,
                TotalCount = items.Count,
                AnalysisType = request.AnalysisType.ToString()
            };
        }

        public async Task<List<GetEmployeePerformanceResponse>> GetEmployeePerformanceAsync(
        GetEmployeePerformanceQuery request,
        CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Employee>().GetAll();

            if (request.EmployeeId.HasValue)
                query = query.Where(e => e.EmployeeId == request.EmployeeId);

            var employees = await query
                .OrderBy(e => e.LastName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new GetEmployeePerformanceResponse
                {
                    EmployeeId = e.EmployeeId,
                    FullName = e.FirstName + " " + e.LastName,
                    Title = e.Title,
                    ReportsToName = e.ReportsToNavigation != null
                        ? e.ReportsToNavigation.FirstName + " " + e.ReportsToNavigation.LastName
                        : null,

                    // Siparis bilgileri
                    TotalOrders = e.Orders
                        .Count(o => o.OrderDate.HasValue &&
                            (!request.Year.HasValue || o.OrderDate.Value.Year == request.Year) &&
                            (!request.Month.HasValue || o.OrderDate.Value.Month == request.Month)),

                    TotalRevenue = e.Orders
                        .Where(o => o.OrderDate.HasValue &&
                            (!request.Year.HasValue || o.OrderDate.Value.Year == request.Year) &&
                            (!request.Month.HasValue || o.OrderDate.Value.Month == request.Month))
                        .SelectMany(o => o.OrderDetails)
                        .Sum(od => (decimal)(od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount))),

                    AverageOrderValue = e.Orders
                        .Where(o => o.OrderDate.HasValue &&
                            (!request.Year.HasValue || o.OrderDate.Value.Year == request.Year) &&
                            (!request.Month.HasValue || o.OrderDate.Value.Month == request.Month))
                        .SelectMany(o => o.OrderDetails)
                        .Any()
                            ? e.Orders
                                .Where(o => o.OrderDate.HasValue &&
                                    (!request.Year.HasValue || o.OrderDate.Value.Year == request.Year) &&
                                    (!request.Month.HasValue || o.OrderDate.Value.Month == request.Month))
                                .SelectMany(o => o.OrderDetails)
                                .Average(od => (decimal)(od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount)))
                            : 0,

                    TotalItemsSold = e.Orders
                        .Where(o => o.OrderDate.HasValue &&
                            (!request.Year.HasValue || o.OrderDate.Value.Year == request.Year) &&
                            (!request.Month.HasValue || o.OrderDate.Value.Month == request.Month))
                        .SelectMany(o => o.OrderDetails)
                        .Sum(od => (int)od.Quantity),

                    // En çok sattigi kategori
                    TopCategory = e.Orders
                        .Where(o => o.OrderDate.HasValue &&
                            (!request.Year.HasValue || o.OrderDate.Value.Year == request.Year) &&
                            (!request.Month.HasValue || o.OrderDate.Value.Month == request.Month))
                        .SelectMany(o => o.OrderDetails)
                        .GroupBy(od => od.Product!.Category!.CategoryName)
                        .OrderByDescending(g => g.Sum(od => od.Quantity))
                        .Select(g => g.Key)
                        .FirstOrDefault(),

                    // En çok sattigi müsteri
                    TopCustomer = e.Orders
                        .Where(o => o.OrderDate.HasValue &&
                            (!request.Year.HasValue || o.OrderDate.Value.Year == request.Year) &&
                            (!request.Month.HasValue || o.OrderDate.Value.Month == request.Month))
                        .GroupBy(o => o.Customer!.CompanyName)
                        .OrderByDescending(g => g.Count())
                        .Select(g => g.Key)
                        .FirstOrDefault(),

                    // Aylik trend
                    MonthlyTrends = e.Orders
                        .Where(o => o.OrderDate.HasValue &&
                            (!request.Year.HasValue || o.OrderDate.Value.Year == request.Year))
                        .GroupBy(o => new
                        {
                            Year = o.OrderDate!.Value.Year,
                            Month = o.OrderDate!.Value.Month
                        })
                        .Select(g => new EmployeeMonthlyTrend
                        {
                            Year = g.Key.Year,
                            Month = g.Key.Month,
                            TotalOrders = g.Count(),
                            TotalRevenue = g.SelectMany(o => o.OrderDetails)
                                .Sum(od => (decimal)(od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount)))
                        })
                        .OrderBy(t => t.Year)
                        .ThenBy(t => t.Month)
                        .ToList()
                })
                .ToListAsync(cancellationToken);


            foreach (var employee in employees)
                foreach (var trend in employee.MonthlyTrends)
                    trend.MonthName = DateHelper.GetMonthName(trend.Year, trend.Month);

            return employees;
        }

        public async Task<GetCustomerRFMResult> GetCustomerRFMAsync(
        GetCustomerRFMQuery request,
        CancellationToken cancellationToken)
        {
            var referenceDate = DateTime.UtcNow;


            var rawData = await _unitOfWork.Repository<Customer>()
                .GetAll()
                .Where(c => c.Orders.Any())
                .Where(c => string.IsNullOrEmpty(request.Country) || c.Country == request.Country)
                .Select(c => new
                {
                    c.CustomerId,
                    c.CompanyName,
                    c.Country,
                    c.City,
                    LastOrderDate = c.Orders
                        .Where(o => o.OrderDate.HasValue)
                        .Max(o => o.OrderDate),
                    TotalOrders = c.Orders.Count(),
                    TotalSpent = c.Orders
                        .SelectMany(o => o.OrderDetails)
                        .Sum(od => (decimal)(od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount)))
                })
                .ToListAsync(cancellationToken);


            var rfmData = rawData.Select(c => new
            {
                c.CustomerId,
                c.CompanyName,
                c.Country,
                c.City,
                DaysSinceLastOrder = c.LastOrderDate.HasValue
                    ? (int)(referenceDate - c.LastOrderDate.Value).TotalDays
                    : int.MaxValue,
                c.TotalOrders,
                c.TotalSpent
            }).ToList();


            var recencyValues = rfmData.Select(x => x.DaysSinceLastOrder).OrderBy(x => x).ToList();
            var frequencyValues = rfmData.Select(x => x.TotalOrders).OrderBy(x => x).ToList();
            var monetaryValues = rfmData.Select(x => x.TotalSpent).OrderBy(x => x).ToList();

            int count = rfmData.Count;


            double r20 = recencyValues[(int)(count * 0.20)];
            double r40 = recencyValues[(int)(count * 0.40)];
            double r60 = recencyValues[(int)(count * 0.60)];
            double r80 = recencyValues[(int)(count * 0.80)];


            double f20 = frequencyValues[(int)(count * 0.20)];
            double f40 = frequencyValues[(int)(count * 0.40)];
            double f60 = frequencyValues[(int)(count * 0.60)];
            double f80 = frequencyValues[(int)(count * 0.80)];


            double m20 = (double)monetaryValues[(int)(count * 0.20)];
            double m40 = (double)monetaryValues[(int)(count * 0.40)];
            double m60 = (double)monetaryValues[(int)(count * 0.60)];
            double m80 = (double)monetaryValues[(int)(count * 0.80)];


            var scored = rfmData.Select(c =>
            {
                // Recency: az gün geçmis = yüksek skor (ters)
                int rScore = c.DaysSinceLastOrder <= r20 ? 5
                    : c.DaysSinceLastOrder <= r40 ? 4
                    : c.DaysSinceLastOrder <= r60 ? 3
                    : c.DaysSinceLastOrder <= r80 ? 2 : 1;

                // Frequency: çok siparis = yüksek skor
                int fScore = c.TotalOrders >= f80 ? 5
                    : c.TotalOrders >= f60 ? 4
                    : c.TotalOrders >= f40 ? 3
                    : c.TotalOrders >= f20 ? 2 : 1;

                // Monetary: çok harcama = yüksek skor
                int mScore = (double)c.TotalSpent >= m80 ? 5
                    : (double)c.TotalSpent >= m60 ? 4
                    : (double)c.TotalSpent >= m40 ? 3
                    : (double)c.TotalSpent >= m20 ? 2 : 1;

                int rfmScore = rScore + fScore + mScore;


                string segment = RFMSegmentCalculator.DetermineSegment(rScore, fScore, mScore);

                return new GetCustomerRFMResponse
                {
                    CustomerId = c.CustomerId,
                    CompanyName = c.CompanyName,
                    Country = c.Country,
                    City = c.City,
                    DaysSinceLastOrder = c.DaysSinceLastOrder,
                    TotalOrders = c.TotalOrders,
                    TotalSpent = c.TotalSpent,
                    RecencyScore = rScore,
                    FrequencyScore = fScore,
                    MonetaryScore = mScore,
                    RFMScore = rfmScore,
                    Segment = segment
                };
            }).ToList();


            var filtered = scored.AsEnumerable();
            if (!string.IsNullOrEmpty(request.Segment))
                filtered = filtered.Where(x => x.Segment == request.Segment);

            var filteredList = filtered
                .OrderByDescending(x => x.RFMScore)
                .ToList();


            var summary = new RFMSummary
            {
                TotalCustomers = scored.Count,
                Champions = scored.Count(x => x.Segment == "Champions"),
                Loyal = scored.Count(x => x.Segment == "Loyal"),
                AtRisk = scored.Count(x => x.Segment == "AtRisk"),
                Lost = scored.Count(x => x.Segment == "Lost"),
                NewCustomers = scored.Count(x => x.Segment == "NewCustomers"),
                Others = scored.Count(x => x.Segment == "Others")
            };


            var paged = filteredList
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return new GetCustomerRFMResult
            {
                Items = paged,
                TotalCount = filteredList.Count,
                Summary = summary
            };
        }
        public async Task<List<GetTopSellingProductsResponse>> GetTopSellingProductsAsync(
        GetTopSellingProductsQuery request,
        CancellationToken cancellationToken)
        {
            var orderQuery = _unitOfWork.Repository<Orders>()
                .GetAll()
                .Where(o => o.OrderDate.HasValue);


            if (request.Year.HasValue)
                orderQuery = orderQuery.Where(o => o.OrderDate!.Value.Year == request.Year.Value);

            if (request.Month.HasValue)
                orderQuery = orderQuery.Where(o => o.OrderDate!.Value.Month == request.Month.Value);


            var currentPeriod = await orderQuery
                .SelectMany(o => o.OrderDetails)
                .Where(od => !request.CategoryId.HasValue || od.Product!.CategoryId == request.CategoryId)
                .GroupBy(od => new
                {
                    od.ProductId,
                    od.Product!.ProductName,
                    od.Product.CategoryId,
                    CategoryName = od.Product.Category!.CategoryName
                })
                .Select(g => new
                {
                    g.Key.ProductId,
                    g.Key.ProductName,
                    g.Key.CategoryId,
                    g.Key.CategoryName,
                    TotalQuantitySold = g.Sum(od => (int)od.Quantity),
                    TotalRevenue = g.Sum(od => (decimal)(od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount))),
                    TotalOrders = g.Select(od => od.OrderId).Distinct().Count()
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);


            var previousOrderQuery = _unitOfWork.Repository<Orders>()
                .GetAll()
                .Where(o => o.OrderDate.HasValue);

            if (request.Year.HasValue && request.Month.HasValue)
            {
                // Ay bazinda ? önceki ay
                var prevDate = new DateTime(request.Year.Value, request.Month.Value, 1).AddMonths(-1);
                previousOrderQuery = previousOrderQuery
                    .Where(o => o.OrderDate!.Value.Year == prevDate.Year &&
                                o.OrderDate!.Value.Month == prevDate.Month);
            }
            else if (request.Year.HasValue)
            {
                // Yil bazinda ? önceki yil
                previousOrderQuery = previousOrderQuery
                    .Where(o => o.OrderDate!.Value.Year == request.Year.Value - 1);
            }
            else
            {

                var maxYear = await _unitOfWork.Repository<Orders>()
                    .GetAll()
                    .Where(o => o.OrderDate.HasValue)
                    .MaxAsync(o => o.OrderDate!.Value.Year, cancellationToken);

                previousOrderQuery = previousOrderQuery
                    .Where(o => o.OrderDate!.Value.Year == maxYear - 1);
            }


            var productIds = currentPeriod.Select(x => x.ProductId).ToList();

            var previousPeriod = await previousOrderQuery
                .SelectMany(o => o.OrderDetails)
                .Where(od => productIds.Contains(od.ProductId))
                .Where(od => !request.CategoryId.HasValue || od.Product!.CategoryId == request.CategoryId)
                .GroupBy(od => od.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantitySold = g.Sum(od => (int)od.Quantity),
                    TotalRevenue = g.Sum(od => (decimal)(od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount)))
                })
                .ToListAsync(cancellationToken);


            return currentPeriod.Select(current =>
            {
                var previous = previousPeriod.FirstOrDefault(p => p.ProductId == current.ProductId);

                decimal? quantityChangePercent = null;
                decimal? revenueChangePercent = null;
                string trendDirection = "New";

                if (previous != null)
                {
                    if (previous.TotalQuantitySold > 0)
                    {
                        quantityChangePercent = Math.Round(
                            ((decimal)(current.TotalQuantitySold - previous.TotalQuantitySold) /
                            previous.TotalQuantitySold) * 100, 2);
                    }

                    if (previous.TotalRevenue > 0)
                    {
                        revenueChangePercent = Math.Round(
                            ((current.TotalRevenue - previous.TotalRevenue) /
                            previous.TotalRevenue) * 100, 2);
                    }

                    trendDirection = quantityChangePercent switch
                    {
                        > 5 => "Up",
                        < -5 => "Down",
                        _ => "Stable"
                    };
                }

                return new GetTopSellingProductsResponse
                {
                    ProductId = current.ProductId,
                    ProductName = current.ProductName,
                    CategoryId = current.CategoryId,
                    CategoryName = current.CategoryName,
                    TotalQuantitySold = current.TotalQuantitySold,
                    TotalRevenue = current.TotalRevenue,
                    TotalOrders = current.TotalOrders,
                    PreviousPeriodQuantity = previous?.TotalQuantitySold,
                    PreviousPeriodRevenue = previous?.TotalRevenue,
                    QuantityChangePercent = quantityChangePercent,
                    RevenueChangePercent = revenueChangePercent,
                    TrendDirection = trendDirection
                };
            }).ToList();
        }


        public async Task<List<GetSalesByProductResponse>> GetSalesByProductAsync(
       GetSalesByProductQuery request,
       CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Orders>()
                .GetAll()
                .AsNoTracking()
                .Where(o => o.OrderDate.HasValue);


            if (request.Year.HasValue && request.Month.HasValue)
            {
                var start = new DateTime(request.Year.Value, request.Month.Value, 1);
                var end = start.AddMonths(1);
                query = query.Where(o => o.OrderDate >= start && o.OrderDate < end);
            }
            else if (request.Year.HasValue)
            {
                var start = new DateTime(request.Year.Value, 1, 1);
                var end = start.AddYears(1);
                query = query.Where(o => o.OrderDate >= start && o.OrderDate < end);
            }

            var salesQuery = query
                .SelectMany(o => o.OrderDetails)
                .Where(od =>
                    !request.CategoryId.HasValue ||
                    od.Product!.CategoryId == request.CategoryId.Value)
                .Select(od => new
                {
                    od.OrderId,
                    od.ProductId,
                    ProductName = od.Product!.ProductName,
                    CategoryId = od.Product.CategoryId,
                    CategoryName = od.Product.Category!.CategoryName,
                    SupplierName = od.Product.Supplier != null
                        ? od.Product.Supplier.CompanyName
                        : null,
                    UnitPrice = od.Product.UnitPrice,
                    UnitsInStock = od.Product.UnitsInStock,

                    Quantity = (int)od.Quantity,
                    Revenue =
                        (decimal)od.Quantity *
                        od.UnitPrice *
                        (1 - (decimal)od.Discount)
                });

            return await salesQuery
                .GroupBy(x => new
                {
                    x.ProductId,
                    x.ProductName,
                    x.CategoryId,
                    x.CategoryName,
                    x.SupplierName,
                    x.UnitPrice,
                    x.UnitsInStock
                })
                .Select(g => new GetSalesByProductResponse
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    SupplierName = g.Key.SupplierName,
                    UnitPrice = g.Key.UnitPrice,
                    UnitsInStock = g.Key.UnitsInStock,

                    TotalOrders = g.Select(x => x.OrderId).Distinct().Count(),
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Revenue),
                    AverageOrderValue = g.Average(x => x.Revenue)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<GetSalesByEmployeeResponse>> GetSalesByEmployeeAsync(
        GetSalesByEmployeeQuery request,
        CancellationToken cancellationToken)
        {
            var orderQuery = _unitOfWork.Repository<Orders>()
                .GetAll()
                .AsNoTracking()
                .Where(o => o.OrderDate.HasValue && o.EmployeeId.HasValue);

            if (request.Year.HasValue && request.Month.HasValue)
            {
                var start = new DateTime(request.Year.Value, request.Month.Value, 1);
                var end = start.AddMonths(1);
                orderQuery = orderQuery.Where(o => o.OrderDate >= start && o.OrderDate < end);
            }
            else if (request.Year.HasValue)
            {
                var start = new DateTime(request.Year.Value, 1, 1);
                var end = start.AddYears(1);
                orderQuery = orderQuery.Where(o => o.OrderDate >= start && o.OrderDate < end);
            }

            if (request.EmployeeId.HasValue)
                orderQuery = orderQuery.Where(o => o.EmployeeId == request.EmployeeId.Value);

            return await orderQuery
                .GroupBy(o => new
                {
                    o.Employee!.EmployeeId,
                    o.Employee.FirstName,
                    o.Employee.LastName,
                    o.Employee.Title,
                    ReportsToName = o.Employee.ReportsToNavigation != null
                        ? o.Employee.ReportsToNavigation.FirstName + " " + o.Employee.ReportsToNavigation.LastName
                        : null
                })
                .Select(g => new GetSalesByEmployeeResponse
                {
                    EmployeeId = g.Key.EmployeeId,
                    FullName = g.Key.FirstName + " " + g.Key.LastName,
                    Title = g.Key.Title,
                    ReportsToName = g.Key.ReportsToName,
                    TotalOrders = g.Count(),
                    TotalItemsSold = g.SelectMany(o => o.OrderDetails)
                        .Sum(od => (int)od.Quantity),
                    TotalRevenue = g.SelectMany(o => o.OrderDetails)
                        .Sum(od => (decimal)(od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount))),
                    AverageOrderValue = g.SelectMany(o => o.OrderDetails)
                        .Average(od => (decimal)(od.Quantity * od.UnitPrice * (decimal)(1 - od.Discount))),
                    TopCategory = g.SelectMany(o => o.OrderDetails)
                        .GroupBy(od => od.Product!.Category!.CategoryName)
                        .OrderByDescending(cg => cg.Sum(od => od.Quantity))
                        .Select(cg => cg.Key)
                        .FirstOrDefault(),
                    TopCustomer = g.GroupBy(o => o.Customer!.CompanyName)
                        .OrderByDescending(cg => cg.Count())
                        .Select(cg => cg.Key)
                        .FirstOrDefault()
                })
                .OrderByDescending(x => x.TotalRevenue)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
        }


    }
}
