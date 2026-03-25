using NorthwindApi.Application.Features.Reports.GetCustomerRFM;
using NorthwindApi.Application.Features.Reports.GetEmployeePerformance;
using NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByCategory;
using NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByPeriod;
using NorthwindApi.Application.Features.Reports.GetStockAnalysis;
using NorthwindApi.Application.Features.Reports.GetTopSellingProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.Services
{
    public interface IReportService
    {
        Task<List<GetSalesByPeriodResponse>> GetSalesByPeriodAsync(GetSalesByPeriodQuery request, CancellationToken cancellationToken);
        Task<List<GetSalesByCategoryResponse>> GetSalesByCategoryAsync(GetSalesByCategoryQuery request, CancellationToken cancellationToken);
        Task<GetStockAnalysisResponse> GetStockAnalysisAsync(GetStockAnalysisQuery request, CancellationToken cancellationToken);
        Task<List<GetEmployeePerformanceResponse>> GetEmployeePerformanceAsync(GetEmployeePerformanceQuery request, CancellationToken cancellationToken);
        Task<GetCustomerRFMResult> GetCustomerRFMAsync(GetCustomerRFMQuery request, CancellationToken cancellationToken);
        Task<List<GetTopSellingProductsResponse>> GetTopSellingProductsAsync(GetTopSellingProductsQuery request, CancellationToken cancellationToken);

    }
}

