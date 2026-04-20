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
    public class SupplierBusinessRules : ISupplierBusinessRules
    {
        private readonly IUnitOfWork _unitOfWork;

        public SupplierBusinessRules(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task SupplierMustExistAsync(int supplierId, CancellationToken cancellationToken = default)
        {
            var exists = await _unitOfWork.Repository<Suppliers>()
                .GetAll()
                .AnyAsync(s => s.SupplierId == supplierId, cancellationToken);

            if (!exists)
                throw new KeyNotFoundException($"Supplier with ID {supplierId} not found.");
        }

        public async Task SupplierCompanyNameMustBeUniqueAsync(string companyName, CancellationToken cancellationToken = default)
        {
            var exists = await _unitOfWork.Repository<Suppliers>()
                .GetAll()
                .AnyAsync(s => s.CompanyName == companyName, cancellationToken);

            if (exists)
                throw new InvalidOperationException($"Supplier with company name '{companyName}' already exists.");
        }

        public async Task SupplierHasNoProductsAsync(int supplierId, CancellationToken cancellationToken = default)
        {
            var hasProducts = await _unitOfWork.Repository<Products>()
                .GetAll()
                .AnyAsync(p => p.SupplierId == supplierId, cancellationToken);

            if (hasProducts)
                throw new InvalidOperationException($"Supplier with ID {supplierId} has products and cannot be deleted.");
        }
    }
}
