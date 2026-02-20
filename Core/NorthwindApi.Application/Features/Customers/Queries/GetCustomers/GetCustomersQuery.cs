using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Customers.Queries.GetCustomers
{
    public class GetCustomersQuery:IRequest<List<GetCustomersQueryResponse>>
    {
    }
}
