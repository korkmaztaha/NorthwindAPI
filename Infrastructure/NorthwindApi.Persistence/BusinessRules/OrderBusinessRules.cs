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
    public class OrderBusinessRules : IOrderBusinessRules
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderBusinessRules(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task OrderMustExistAsync(int orderId, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Repository<Orders>()
                .GetAll()
                .AnyAsync(x => x.OrderId == orderId, cancellationToken);

            if (!exists)
                throw new KeyNotFoundException($"{orderId} ID'li sipariş bulunamadı.");
        }
    }
}
