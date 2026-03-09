using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetStockAnalysis
{
    public class GetStockAnalysisQueryHandler : IRequestHandler<GetStockAnalysisQuery, GetStockAnalysisResponse>
    {
        private readonly IReportService _reportService;

        public GetStockAnalysisQueryHandler(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<GetStockAnalysisResponse> Handle(
            GetStockAnalysisQuery request,
            CancellationToken cancellationToken)
            => await _reportService.GetStockAnalysisAsync(request, cancellationToken);
    }
}
