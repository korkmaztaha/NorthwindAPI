using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetSalesByProduct
{
    public class GetSalesByProductQuery : IRequest<List<GetSalesByProductResponse>>
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? CategoryId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
