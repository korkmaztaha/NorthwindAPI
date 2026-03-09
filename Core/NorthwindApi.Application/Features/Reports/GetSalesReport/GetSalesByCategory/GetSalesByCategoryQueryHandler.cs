using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByCategory
{
    public class GetSalesByCategoryQueryHandler : IRequestHandler<GetSalesByCategoryQuery, List<GetSalesByCategoryResponse>>
    {
        private readonly IReportService _reportService;

        public GetSalesByCategoryQueryHandler(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<List<GetSalesByCategoryResponse>> Handle(
            GetSalesByCategoryQuery request,
            CancellationToken cancellationToken)
            => await _reportService.GetSalesByCategoryAsync(request, cancellationToken);
    }
}
