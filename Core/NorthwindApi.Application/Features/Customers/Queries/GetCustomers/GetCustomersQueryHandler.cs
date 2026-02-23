using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NorthwindApi.Application.Interfaces;
using NorthwindApi.Domain.Entities;


namespace NorthwindApi.Application.Features.Customers.Queries.GetCustomers
{
    public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, List<GetCustomersQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetCustomersQueryHandler> _logger;

        public GetCustomersQueryHandler(IUnitOfWork unitOfWork, ILogger<GetCustomersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<GetCustomersQueryResponse>> Handle(
            GetCustomersQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Müşteriler getiriliyor");
            var result = await _unitOfWork.Repository<Customer>()
                .GetAll()
                .Select(x => new GetCustomersQueryResponse
                {
                    CustomerId = x.CustomerId,
                    CompanyName = x.CompanyName,
                    City = x.City
                })
                .ToListAsync(cancellationToken);
            _logger.LogInformation("{Count} müşteri getirildi", result.Count);

            return result;
        }
    }
}