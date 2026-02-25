using NorthwindApi.Application.Features.Customers.Commands.CreateCustomer;
using NorthwindApi.Application.Features.Customers.Commands.UpdateCustomer;
using NorthwindApi.Application.Features.Customers.Queries.GetCustomerOrderSummary;
using NorthwindApi.Application.Features.Customers.Queries.GetCustomers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<List<GetCustomersQueryResponse>> GetAllAsync(GetCustomersQuery request, CancellationToken cancellationToken);
        Task<CreateCustomerCommandResponse> CreateAsync(CreateCustomerCommand request, CancellationToken cancellationToken);
        Task<UpdateCustomerCommandResponse> UpdateAsync(UpdateCustomerCommand request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(string customerId, CancellationToken cancellationToken);
        Task<List<GetCustomerOrderSummaryResponse>> GetOrderSummaryAsync(GetCustomerOrderSummaryQuery request,CancellationToken cancellationToken);
    }
}

