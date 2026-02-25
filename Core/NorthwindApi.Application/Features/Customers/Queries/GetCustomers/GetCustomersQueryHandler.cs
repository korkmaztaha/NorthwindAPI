using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NorthwindApi.Application.Interfaces;
using NorthwindApi.Domain.Constants;
using NorthwindApi.Domain.Entities;


namespace NorthwindApi.Application.Features.Customers.Queries.GetCustomers
{
    public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, List<GetCustomersQueryResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetCustomersQueryHandler> _logger;


        public GetCustomersQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService, ILogger<GetCustomersQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<GetCustomersQueryResponse>> Handle(GetCustomersQuery request,CancellationToken cancellationToken)
        {
            try
            {
                var query = _unitOfWork.Repository<Customer>().GetAll();

                if (!string.IsNullOrEmpty(request.CustomerId))
                    query = query.Where(x => x.CustomerId == request.CustomerId);

                if (!string.IsNullOrEmpty(request.City))
                    query = query.Where(x => x.City == request.City);

                if (!string.IsNullOrEmpty(request.Country))
                    query = query.Where(x => x.Country == request.Country);

                if (!string.IsNullOrEmpty(request.CompanyName))
                    query = query.Where(x => x.CompanyName.Contains(request.CompanyName));

                var items = await query
                   .OrderBy(x => x.CompanyName)
                   .Skip((request.PageNumber - 1) * request.PageSize)
                   .Take(request.PageSize)
                       .Select(x => new GetCustomersQueryResponse
                       {
                           CustomerId = x.CustomerId,
                           CompanyName = x.CompanyName,
                           ContactName=x.ContactName,
                           ContactTitle = x.ContactTitle,
                           City = x.City,
                           Country = x.Country,
                           Phone = x.Phone,
                           Fax = x.Fax,
                           Address = x.Address

                       })
                    .ToListAsync(cancellationToken);

                return items;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müşteriler getirilirken hata oluştu.");
                throw;
            }
        }
    }
}