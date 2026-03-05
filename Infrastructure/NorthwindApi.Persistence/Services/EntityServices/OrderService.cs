using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Features.Orders.Queries.GetOrderDetail;
using NorthwindApi.Application.Features.Orders.Queries.GetOrders;
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
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<GetOrdersQueryResponse>> GetAllAsync(
          GetOrdersQuery request,
          CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Orders>().GetAll();

            if (!string.IsNullOrEmpty(request.CustomerId))
                query = query.Where(x => x.CustomerId == request.CustomerId);



            if (request.EmployeeId.HasValue)
                query = query.Where(x => x.EmployeeId == request.EmployeeId);


            if (!string.IsNullOrEmpty(request.EmployeeName))
                query = query.Where(x =>
                    x.Employee != null && (
                    x.Employee.FirstName.Contains(request.EmployeeName) ||
                    x.Employee.LastName.Contains(request.EmployeeName)));



            if (request.StartDate.HasValue)
                query = query.Where(x => x.OrderDate >= request.StartDate);

            if (request.EndDate.HasValue)
                query = query.Where(x => x.OrderDate <= request.EndDate);

            if (!string.IsNullOrEmpty(request.ShipCountry))
                query = query.Where(x => x.ShipCountry == request.ShipCountry);

            return await query
                .OrderByDescending(x => x.OrderDate)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new GetOrdersQueryResponse
                {
                    OrderId = x.OrderId,

                    // Müşteri bilgisi
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer != null ? x.Customer.CompanyName : null,
                    CustomerCity = x.Customer != null ? x.Customer.City : null,

                    // Çalışan bilgisi
                    EmployeeId = x.EmployeeId,
                    EmployeeFullName = x.Employee != null
                        ? x.Employee.FirstName + " " + x.Employee.LastName
                        : null,
                    EmployeeTitle = x.Employee != null ? x.Employee.Title : null,

                    // Kargo bilgisi
                    ShipperName = x.ShipViaNavigation != null
                        ? x.ShipViaNavigation.CompanyName
                        : null,
                    ShipName = x.ShipName,
                    ShipCity = x.ShipCity,
                    ShipCountry = x.ShipCountry,

                    // Tarih bilgisi
                    OrderDate = x.OrderDate,
                    RequiredDate = x.RequiredDate,
                    ShippedDate = x.ShippedDate,
                    Freight = x.Freight,

                    // Hesaplanan alanlar
                    TotalAmount = x.OrderDetails
                        .Sum(od => (od.Quantity * od.UnitPrice * (1 - (decimal)od.Discount))),
                    TotalItems = x.OrderDetails.Sum(od => od.Quantity)
                })
                .ToListAsync(cancellationToken);
        }


        public async Task<GetOrderDetailResponse> GetDetailAsync(
       int orderId,
       CancellationToken cancellationToken)
        {
            var order = await _unitOfWork.Repository<Orders>()
                .GetAll()
                .Where(x => x.OrderId == orderId)
                .Select(x => new GetOrderDetailResponse
                {
                    OrderId = x.OrderId,
                    CustomerId = x.CustomerId,
                    CustomerName = x.Customer != null ? x.Customer.CompanyName : null,
                    EmployeeFullName = x.Employee != null
                        ? x.Employee.FirstName + " " + x.Employee.LastName
                        : null,
                    OrderDate = x.OrderDate,
                    ShippedDate = x.ShippedDate,
                    ShipperName = x.ShipViaNavigation != null
                        ? x.ShipViaNavigation.CompanyName
                        : null,
                    ShipAddress = x.ShipAddress,
                    ShipCity = x.ShipCity,
                    ShipCountry = x.ShipCountry,
                    Freight = x.Freight,

                    // Toplam tutar
                    TotalAmount = x.OrderDetails
                        .Sum(od => (od.Quantity * od.UnitPrice * (1 - (decimal)od.Discount))),

                    // Ürün detayları
                    Items = x.OrderDetails.Select(od => new OrderDetailItemResponse
                    {
                        ProductId = od.ProductId,
                        ProductName = od.Product != null ? od.Product.ProductName : null!,
                        CategoryName = od.Product != null && od.Product.Category != null
                            ? od.Product.Category.CategoryName
                            : null,
                        UnitPrice = od.UnitPrice,
                        Quantity = od.Quantity,
                        Discount = od.Discount,
                        LineTotal = (od.Quantity * od.UnitPrice * (1 - (decimal)od.Discount))
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (order is null)
                throw new KeyNotFoundException($"{orderId} ID'li sipariş bulunamadı.");

            return order;
        }
    }
}
