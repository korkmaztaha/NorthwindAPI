using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByPeriod;
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
                item.MonthName = new DateTime(item.Year, item.Month!.Value, 1)
                    .ToString("MMMM", new System.Globalization.CultureInfo("tr-TR"));

            return result;
        }
    }
}
