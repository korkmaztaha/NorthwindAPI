using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetCustomerRFM
{
    public class GetCustomerRFMQuery : IRequest<GetCustomerRFMResult>
    {
        public string? Segment { get; set; } // Champions, Loyal, AtRisk, Lost, NewCustomers
        public string? Country { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
