using MediatR;
using Microsoft.EntityFrameworkCore;
using NorthwindApi.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Customers.Queries.GetCustomers
{
    public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, List<GetCustomersQueryResponse>>
    {
        private readonly NorthwindDbContext _context;

        public GetCustomersQueryHandler(NorthwindDbContext context)
        {
            _context = context;
        }

        public async Task<List<GetCustomersQueryResponse>> Handle(
         GetCustomersQuery request,
         CancellationToken cancellationToken)
        {
            var result = await _context.Customers
                .AsNoTracking()
                .Select(x => new GetCustomersQueryResponse
                {
                    CustomerId = x.CustomerId,
                    CompanyName = x.CompanyName,
                    City = x.City
                })
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
