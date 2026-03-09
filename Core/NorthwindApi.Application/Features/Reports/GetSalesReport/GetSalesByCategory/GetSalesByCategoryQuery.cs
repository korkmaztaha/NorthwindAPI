using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByCategory
{
    public class GetSalesByCategoryQuery : IRequest<List<GetSalesByCategoryResponse>>
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
