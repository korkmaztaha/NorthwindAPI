using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByPeriod
{
    public class GetSalesByPeriodQueryHandler : IRequestHandler<GetSalesByPeriodQuery, List<GetSalesByPeriodResponse>>
    {
        private readonly IReportService _reportService;

        public GetSalesByPeriodQueryHandler(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<List<GetSalesByPeriodResponse>> Handle(
            GetSalesByPeriodQuery request,
            CancellationToken cancellationToken)
            => await _reportService.GetSalesByPeriodAsync(request, cancellationToken);
    }
}
