using MediatR;
using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Interfaces;
using NorthwindApi.Domain.Entities;


namespace NorthwindApi.Application.Features.Customers.Queries.GetCustomers
{
    public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, List<GetCustomersQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCustomersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<GetCustomersQueryResponse>> Handle(
            GetCustomersQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Repository<Customer>()
                .GetAll()
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