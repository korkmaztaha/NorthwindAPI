using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetSalesByProduct
{
    public class GetSalesByProductQueryHandler : IRequestHandler<GetSalesByProductQuery, List<GetSalesByProductResponse>>
    {
        private readonly IReportService _reportService;

        public GetSalesByProductQueryHandler(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<List<GetSalesByProductResponse>> Handle(
            GetSalesByProductQuery request,
            CancellationToken cancellationToken)
            => await _reportService.GetSalesByProductAsync(request, cancellationToken);
    }
}
