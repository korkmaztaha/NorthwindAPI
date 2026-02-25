using MediatR;
using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Interfaces;
using NorthwindApi.Domain.Entities;

namespace NorthwindApi.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CreateCustomerCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateCustomerCommandResponse> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await _unitOfWork.Repository<Customer>()
            .GetAll()
            .AnyAsync(x => x.CustomerId == request.CustomerId, cancellationToken);

        if (exists)
            throw new InvalidOperationException($"{request.CustomerId} ID'li müşteri zaten mevcut.");

        var customer = new Customer
        {
            CustomerId = request.CustomerId.ToUpper(),
            CompanyName = request.CompanyName,
            ContactName = request.ContactName,
            ContactTitle = request.ContactTitle,
            Address = request.Address,
            City = request.City,
            Country = request.Country,
            Phone = request.Phone,
            Fax = request.Fax
        };

        await _unitOfWork.Repository<Customer>().AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateCustomerCommandResponse
        {
            CustomerId = customer.CustomerId,
            CompanyName = customer.CompanyName,
            CreatedAt = DateTime.UtcNow
        };
    }
}