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

namespace NorthwindApi.Application.Features.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, UpdateCustomerCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateCustomerCommandHandler> _logger;

        public UpdateCustomerCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdateCustomerCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<UpdateCustomerCommandResponse> Handle(
            UpdateCustomerCommand request,
            CancellationToken cancellationToken)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müşteri güncellenirken hata oluştu. CustomerId: {CustomerId}", request.CustomerId);
                throw;
            }
        }
    }
}
