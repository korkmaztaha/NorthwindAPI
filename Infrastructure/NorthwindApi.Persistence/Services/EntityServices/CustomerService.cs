using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Features.Customers.Commands.CreateCustomer;
using NorthwindApi.Application.Features.Customers.Commands.UpdateCustomer;
using NorthwindApi.Application.Features.Customers.Queries.GetCustomerOrderSummary;
using NorthwindApi.Application.Features.Customers.Queries.GetCustomers;
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
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<GetCustomersQueryResponse>> GetAllAsync(
            GetCustomersQuery request,
            CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Customer>().GetAll();

            if (!string.IsNullOrEmpty(request.CustomerId))
                query = query.Where(x => x.CustomerId == request.CustomerId);

            if (!string.IsNullOrEmpty(request.City))
                query = query.Where(x => x.City == request.City);

            if (!string.IsNullOrEmpty(request.Country))
                query = query.Where(x => x.Country == request.Country);

            if (!string.IsNullOrEmpty(request.CompanyName))
                query = query.Where(x => x.CompanyName.Contains(request.CompanyName));

            return await query
                .OrderBy(x => x.CompanyName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new GetCustomersQueryResponse
                {
                    CustomerId = x.CustomerId,
                    CompanyName = x.CompanyName,
                    ContactName = x.ContactName,
                    ContactTitle = x.ContactTitle,
                    City = x.City,
                    Country = x.Country,
                    Phone = x.Phone,
                    Fax = x.Fax,
                    Address = x.Address
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<CreateCustomerCommandResponse> CreateAsync(
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

        public async Task<UpdateCustomerCommandResponse> UpdateAsync(
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

        public async Task<bool> DeleteAsync(
            string customerId,
            CancellationToken cancellationToken)
        {
            var customer = await _unitOfWork.Repository<Customer>()
                .GetAll()
                .FirstOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);

            if (customer is null)
                throw new KeyNotFoundException($"{customerId} ID'li müşteri bulunamadı.");

            _unitOfWork.Repository<Customer>().Delete(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<List<GetCustomerOrderSummaryResponse>> GetOrderSummaryAsync(
      GetCustomerOrderSummaryQuery request,
      CancellationToken cancellationToken)
        {
            var cutoffDate = request.LastDays.HasValue
                ? DateTime.UtcNow.AddDays(-request.LastDays.Value)
                : (DateTime?)null;

            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;

            var query = _unitOfWork.Repository<Customer>()
                .GetAll()
                .AsNoTracking()
                .Select(c => new GetCustomerOrderSummaryResponse
                {
                    CustomerId = c.CustomerId,
                    CompanyName = c.CompanyName,
                    City = c.City,
                    Country = c.Country,
                    TotalSpent = c.Orders
                        .Where(o => !cutoffDate.HasValue || o.OrderDate >= cutoffDate)
                        .SelectMany(o => o.OrderDetails)
                        .Sum(od => od.Quantity * od.UnitPrice * (1 - (decimal)od.Discount)),
                    OrderCount = c.Orders
                        .Count(o => !cutoffDate.HasValue || o.OrderDate >= cutoffDate),
                    LastOrderDate = c.Orders
                        .OrderByDescending(o => o.OrderDate)
                        .Select(o => o.OrderDate)
                        .FirstOrDefault(),
                    HasOrders = c.Orders.Any()
                });

            if (request.MinOrderCount.HasValue)
                query = query.Where(x => x.OrderCount >= request.MinOrderCount.Value);

            return await query
                .OrderByDescending(x => x.TotalSpent)
                .Skip((pageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
        }
    }
}