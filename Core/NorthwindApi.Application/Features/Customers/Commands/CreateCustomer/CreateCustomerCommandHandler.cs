using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NorthwindApi.Application.Interfaces;
using NorthwindApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CreateCustomerCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateCustomerCommandHandler> _logger;

        public CreateCustomerCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateCustomerCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CreateCustomerCommandResponse> Handle(
            CreateCustomerCommand request,
            CancellationToken cancellationToken)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müşteri eklenirken hata oluştu. CustomerId: {CustomerId}", request.CustomerId);
                throw;
            }
        }
    }
}
