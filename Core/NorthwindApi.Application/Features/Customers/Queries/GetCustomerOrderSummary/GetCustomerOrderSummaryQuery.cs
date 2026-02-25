using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Customers.Queries.GetCustomerOrderSummary
{
    public class GetCustomerOrderSummaryQuery : IRequest<List<GetCustomerOrderSummaryResponse>>
    {
        public int? LastDays { get; set; }    
        public int? MinOrderCount { get; set; } 
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
