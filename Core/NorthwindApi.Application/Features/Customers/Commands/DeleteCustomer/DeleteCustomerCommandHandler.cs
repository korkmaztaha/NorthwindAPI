using MediatR;
using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Interfaces;
using NorthwindApi.Domain.Entities;

namespace NorthwindApi.Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCustomerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(
        DeleteCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Repository<Customer>()
            .GetAll()
            .FirstOrDefaultAsync(x => x.CustomerId == request.CustomerId, cancellationToken);

        if (customer is null)
            throw new KeyNotFoundException($"{request.CustomerId} ID'li müşteri bulunamadı.");

        _unitOfWork.Repository<Customer>().Delete(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}