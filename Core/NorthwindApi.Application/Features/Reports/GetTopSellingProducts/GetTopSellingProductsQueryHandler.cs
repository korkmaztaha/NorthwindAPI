using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetTopSellingProducts
{
    public class GetTopSellingProductsQueryHandler : IRequestHandler<GetTopSellingProductsQuery, List<GetTopSellingProductsResponse>>
    {
        private readonly IReportService _reportService;

        public GetTopSellingProductsQueryHandler(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<List<GetTopSellingProductsResponse>> Handle(
            GetTopSellingProductsQuery request,
            CancellationToken cancellationToken)
            => await _reportService.GetTopSellingProductsAsync(request, cancellationToken);
    }
}
