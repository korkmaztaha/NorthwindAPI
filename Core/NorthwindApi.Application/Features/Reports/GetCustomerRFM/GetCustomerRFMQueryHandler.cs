using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetCustomerRFM
{
    public class GetCustomerRFMQueryHandler : IRequestHandler<GetCustomerRFMQuery, GetCustomerRFMResult>
    {
        private readonly IReportService _reportService;

        public GetCustomerRFMQueryHandler(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<GetCustomerRFMResult> Handle(
            GetCustomerRFMQuery request,
            CancellationToken cancellationToken)
            => await _reportService.GetCustomerRFMAsync(request, cancellationToken);
    }
}
