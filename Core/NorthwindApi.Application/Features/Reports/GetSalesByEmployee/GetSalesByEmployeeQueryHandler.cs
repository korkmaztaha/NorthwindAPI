using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetSalesByEmployee
{
    public class GetSalesByEmployeeQueryHandler : IRequestHandler<GetSalesByEmployeeQuery, List<GetSalesByEmployeeResponse>>
    {
        private readonly IReportService _reportService;

        public GetSalesByEmployeeQueryHandler(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<List<GetSalesByEmployeeResponse>> Handle(
            GetSalesByEmployeeQuery request,
            CancellationToken cancellationToken)
            => await _reportService.GetSalesByEmployeeAsync(request, cancellationToken);
    }
}
