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
    public class ShipperBusinessRules : IShipperBusinessRules
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShipperBusinessRules(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ShipperMustExistAsync(
            int shipperId, 
            CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Repository<Shippers>()
                .GetAll()
                .AnyAsync(x => x.ShipperId == shipperId, cancellationToken);

            if (!exists)
                throw new KeyNotFoundException($"{shipperId} ID'li kargo firması bulunamadı.");
        }

        public async Task ShipperCompanyNameMustBeUniqueAsync(
            string companyName,
            CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Repository<Shippers>()
                .GetAll()
                .AnyAsync(x => x.CompanyName == companyName, cancellationToken);

            if (exists)
                throw new InvalidOperationException($"{companyName} adlı kargo firması zaten mevcut.");
        }

        public async Task ShipperHasNoOrdersAsync(
            int shipperId, 
            CancellationToken cancellationToken)
        {
            var hasOrders = await _unitOfWork.Repository<Orders>()
                .GetAll()
                .AnyAsync(x => x.ShipVia == shipperId, cancellationToken);

            if (hasOrders)
                throw new InvalidOperationException(
                    $"{shipperId} ID'li kargo firmasına ait siparişler var, silinemez.");
        }
    }
}
