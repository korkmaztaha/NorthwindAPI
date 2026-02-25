using MediatR;
using NorthwindApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Customers.Queries.GetCustomers
{
    public class GetCustomersQuery:IRequest<List<GetCustomersQueryResponse>>
    {
        public string? CustomerId { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? CompanyName { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
