using MediatR;
using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Interfaces;
using NorthwindApi.Domain.Entities;

namespace NorthwindApi.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, UpdateCustomerCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateCustomerCommandResponse> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Repository<Customer>()
            .GetAll()
            .FirstOrDefaultAsync(x => x.CustomerId == request.CustomerId, cancellationToken);

        if (customer is null)
            throw new KeyNotFoundException($"{request.CustomerId} ID'li müşteri bulunamadı.");

        customer.CompanyName = request.CompanyName;
        customer.ContactName = request.ContactName;
        customer.ContactTitle = request.ContactTitle;
        customer.Address = request.Address;
        customer.City = request.City;
        customer.Country = request.Country;
        customer.Phone = request.Phone;
        customer.Fax = request.Fax;

        _unitOfWork.Repository<Customer>().Update(customer);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateCustomerCommandResponse
        {
            CustomerId = customer.CustomerId,
            CompanyName = customer.CompanyName,
            UpdatedAt = DateTime.UtcNow
        };
    }
}