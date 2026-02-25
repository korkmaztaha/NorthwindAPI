using MediatR;
using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Application.Interfaces.Services;
using NorthwindApi.Domain.Entities;

namespace NorthwindApi.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, UpdateCustomerCommandResponse>
{
    private readonly ICustomerService _customerService;

    public UpdateCustomerCommandHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task<UpdateCustomerCommandResponse> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken)
        => await _customerService.UpdateAsync(request, cancellationToken);
}