using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetEmployeePerformance
{
    public class GetEmployeePerformanceQueryHandler : IRequestHandler<GetEmployeePerformanceQuery, List<GetEmployeePerformanceResponse>>
    {
        private readonly IReportService _reportService;

        public GetEmployeePerformanceQueryHandler(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<List<GetEmployeePerformanceResponse>> Handle(
            GetEmployeePerformanceQuery request,
            CancellationToken cancellationToken)
            => await _reportService.GetEmployeePerformanceAsync(request, cancellationToken);
    }
}
