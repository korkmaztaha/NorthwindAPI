using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Application.Interfaces.Services;
using NorthwindApi.Domain.Constants;
using NorthwindApi.Domain.Entities;


namespace NorthwindApi.Application.Features.Customers.Queries.GetCustomers
{
    public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, List<GetCustomersQueryResponse>>
    {
        private readonly ICustomerService _customerService;

        public GetCustomersQueryHandler(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<List<GetCustomersQueryResponse>> Handle(
            GetCustomersQuery request,
            CancellationToken cancellationToken)
            => await _customerService.GetAllAsync(request, cancellationToken);
    }
}