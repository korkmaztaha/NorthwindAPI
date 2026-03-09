using MediatR;
using NorthwindApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByPeriod
{

    public class GetSalesByPeriodQuery : IRequest<List<GetSalesByPeriodResponse>>
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public GroupBy GroupBy { get; set; } = GroupBy.Month;
    }
}
