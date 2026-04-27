using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetSalesByEmployee
{
    public class GetSalesByEmployeeQuery : IRequest<List<GetSalesByEmployeeResponse>>
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? EmployeeId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
