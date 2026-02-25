using MediatR;
using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Features.Customers.Queries.GetCustomers;
using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Application.Interfaces.Services;
using NorthwindApi.Domain.Entities;

namespace NorthwindApi.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CreateCustomerCommandResponse>
{
    private readonly ICustomerService _customerService;

    public CreateCustomerCommandHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task<CreateCustomerCommandResponse> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
        => await _customerService.CreateAsync(request, cancellationToken);
}