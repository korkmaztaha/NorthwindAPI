using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Features.Suppliers.Commands.CreateSupplier;
using NorthwindApi.Application.Features.Suppliers.Commands.DeleteSupplier;
using NorthwindApi.Application.Features.Suppliers.Commands.UpdateSupplier;
using NorthwindApi.Application.Features.Suppliers.Queries.GetSuppliers;
using NorthwindApi.Application.Interfaces.BusinessRules;
using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Application.Interfaces.Services;
using NorthwindApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Persistence.Services.EntityServices
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISupplierBusinessRules _businessRules;

        public SupplierService(IUnitOfWork unitOfWork, ISupplierBusinessRules businessRules)
        {
            _unitOfWork = unitOfWork;
            _businessRules = businessRules;
        }

        public async Task<List<GetSuppliersResponse>> GetSuppliersAsync(
            GetSuppliersQuery request,
            CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Suppliers>().GetAll();

            if (!string.IsNullOrEmpty(request.CompanyName))
                query = query.Where(s => s.CompanyName.Contains(request.CompanyName));

            if (!string.IsNullOrEmpty(request.Country))
                query = query.Where(s => s.Country == request.Country);

            if (!string.IsNullOrEmpty(request.ContactName))
                query = query.Where(s => s.ContactName!.Contains(request.ContactName));

            return await query
                .OrderBy(s => s.CompanyName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new GetSuppliersResponse
                {
                    SupplierId = s.SupplierId,
                    CompanyName = s.CompanyName,
                    ContactName = s.ContactName,
                    ContactTitle = s.ContactTitle,
                    Address = s.Address,
                    City = s.City,
                    Country = s.Country,
                    Phone = s.Phone,
                    Fax = s.Fax,
                    TotalProducts = s.Products.Count()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<CreateSupplierResponse> CreateSupplierAsync(
            CreateSupplierCommand request,
            CancellationToken cancellationToken)
        {
            await _businessRules.SupplierCompanyNameMustBeUniqueAsync(request.CompanyName, cancellationToken);

            var supplier = new Suppliers
            {
                CompanyName = request.CompanyName,
                ContactName = request.ContactName,
                ContactTitle = request.ContactTitle,
                Address = request.Address,
                City = request.City,
                Region = request.Region,
                PostalCode = request.PostalCode,
                Country = request.Country,
                Phone = request.Phone,
                Fax = request.Fax,
                HomePage = request.HomePage
            };

            await _unitOfWork.Repository<Suppliers>().AddAsync(supplier, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateSupplierResponse
            {
                SupplierId = supplier.SupplierId,
                CompanyName = supplier.CompanyName
            };
        }

        public async Task<UpdateSupplierResponse> UpdateSupplierAsync(
            UpdateSupplierCommand request,
            CancellationToken cancellationToken)
        {
            await _businessRules.SupplierMustExistAsync(request.SupplierId, cancellationToken);

            var supplier = await _unitOfWork.Repository<Suppliers>()
                .GetAll()
                .FirstAsync(s => s.SupplierId == request.SupplierId, cancellationToken);

            supplier.CompanyName = request.CompanyName;
            supplier.ContactName = request.ContactName;
            supplier.ContactTitle = request.ContactTitle;
            supplier.Address = request.Address;
            supplier.City = request.City;
            supplier.Region = request.Region;
            supplier.PostalCode = request.PostalCode;
            supplier.Country = request.Country;
            supplier.Phone = request.Phone;
            supplier.Fax = request.Fax;
            supplier.HomePage = request.HomePage;

            _unitOfWork.Repository<Suppliers>().Update(supplier);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateSupplierResponse
            {
                SupplierId = supplier.SupplierId,
                CompanyName = supplier.CompanyName
            };
        }

        public async Task DeleteSupplierAsync(
            DeleteSupplierCommand request,
            CancellationToken cancellationToken)
        {
            await _businessRules.SupplierMustExistAsync(request.SupplierId, cancellationToken);
            await _businessRules.SupplierHasNoProductsAsync(request.SupplierId, cancellationToken);

            var supplier = await _unitOfWork.Repository<Suppliers>()
                .GetAll()
                .FirstAsync(s => s.SupplierId == request.SupplierId, cancellationToken);

            _unitOfWork.Repository<Suppliers>().Delete(supplier);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
