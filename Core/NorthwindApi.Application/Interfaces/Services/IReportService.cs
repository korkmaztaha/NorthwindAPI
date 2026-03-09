using NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByPeriod;
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
    }
}
