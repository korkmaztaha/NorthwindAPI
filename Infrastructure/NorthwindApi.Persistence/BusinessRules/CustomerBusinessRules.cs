using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Interfaces.BusinessRules;
using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Persistence.BusinessRules
{
    public class CustomerBusinessRules : ICustomerBusinessRules
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerBusinessRules(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CustomerMustExistAsync(
            string customerId,
            CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Repository<Customer>()
                .GetAll()
                .AnyAsync(x => x.CustomerId == customerId, cancellationToken);

            if (!exists)
                throw new KeyNotFoundException($"{customerId} ID'li müşteri bulunamadı.");
        }

        public async Task CustomerIdMustBeUniqueAsync(
            string customerId,
            CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Repository<Customer>()
                .GetAll()
                .AnyAsync(x => x.CustomerId == customerId, cancellationToken);

            if (exists)
                throw new InvalidOperationException($"{customerId} ID'li müşteri zaten mevcut.");
        }
    }
}
