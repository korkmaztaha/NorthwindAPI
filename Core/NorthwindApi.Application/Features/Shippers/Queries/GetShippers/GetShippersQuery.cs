using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Shippers.Queries.GetShippers
{

    public class GetShippersQuery : IRequest<List<GetShippersQueryResponse>>
    {
        public string? CompanyName { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
