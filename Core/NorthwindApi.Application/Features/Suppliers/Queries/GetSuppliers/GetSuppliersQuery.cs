using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Suppliers.Queries.GetSuppliers
{
    public class GetSuppliersQuery : IRequest<List<GetSuppliersResponse>>
    {
        public string? CompanyName { get; set; }
        public string? Country { get; set; }
        public string? ContactName { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
