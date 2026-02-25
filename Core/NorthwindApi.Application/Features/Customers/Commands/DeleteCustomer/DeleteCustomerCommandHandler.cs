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

namespace NorthwindApi.Application.Features.Customers.Commands.DeleteCustomer
{
    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteCustomerCommandHandler> _logger;

        public DeleteCustomerCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<DeleteCustomerCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(
            DeleteCustomerCommand request,
            CancellationToken cancellationToken)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Müşteri silinirken hata oluştu. CustomerId: {CustomerId}", request.CustomerId);
                throw;
            }
        }
    }
}
