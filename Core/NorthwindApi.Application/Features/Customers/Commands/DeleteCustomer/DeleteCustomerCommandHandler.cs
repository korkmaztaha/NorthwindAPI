using MediatR;
using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Application.Interfaces.Services;
using NorthwindApi.Domain.Entities;

namespace NorthwindApi.Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly ICustomerService _customerService;

    public DeleteCustomerCommandHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task<bool> Handle(
        DeleteCustomerCommand request,
        CancellationToken cancellationToken)
        => await _customerService.DeleteAsync(request.CustomerId, cancellationToken);
}