using Hangfire;
using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Events;
using NorthwindApi.Application.Features.Orders.Commands.CreateOrder;
using NorthwindApi.Application.Features.Orders.Queries.GetOrderDetail;
using NorthwindApi.Application.Features.Orders.Queries.GetOrders;
using NorthwindApi.Application.Interfaces.BusinessRules;
using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Application.Interfaces.Services;
using NorthwindApi.Domain.Entities;
using NorthwindApi.Persistence.BusinessRules;
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
        private readonly ICustomerBusinessRules _customerBusinessRules;
        private readonly IProductBusinessRules _productBusinessRules;
        private readonly IOrderBusinessRules _orderBusinessRules;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IOutboxService _outboxService;

        public OrderService(
        IUnitOfWork unitOfWork,
        ICustomerBusinessRules customerBusinessRules,
        IProductBusinessRules productBusinessRules,
        IOrderBusinessRules orderBusinessRules,
        IBackgroundJobClient backgroundJobClient,
        IOutboxService outboxService)
        {
            _unitOfWork = unitOfWork;
            _customerBusinessRules = customerBusinessRules;
            _productBusinessRules = productBusinessRules;
            _orderBusinessRules = orderBusinessRules;
            _backgroundJobClient = backgroundJobClient;
            _outboxService = outboxService;
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

            if (request.ShipperId.HasValue)
                query = query.Where(x => x.ShipVia == request.ShipperId);

            if (request.IsDelayed == true)
                query = query.Where(x =>
                    x.RequiredDate < DateTime.UtcNow &&
                    x.ShippedDate == null);

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

        public async Task<CreateOrderCommandResponse> CreateAsync(
             CreateOrderCommand request,
             CancellationToken cancellationToken)
        {
            await _customerBusinessRules.CustomerMustExistAsync(request.CustomerId, cancellationToken);
            var products = await _productBusinessRules.GetAndValidateProductsAsync(request.Items, cancellationToken);
            return await ProcessOrderAsync(request, products, cancellationToken);
        }
        private async Task<CreateOrderCommandResponse> ProcessOrderAsync(
            CreateOrderCommand request,
            List<Products> products,
            CancellationToken cancellationToken)
        {
            var customer = await _unitOfWork.Repository<Customer>()
                .GetAll()
                .FirstAsync(c => c.CustomerId == request.CustomerId, cancellationToken);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var order = new Orders
                {
                    CustomerId = request.CustomerId,
                    EmployeeId = request.EmployeeId,
                    OrderDate = DateTime.UtcNow,
                    RequiredDate = request.RequiredDate,
                    ShipVia = request.ShipVia,
                    Freight = request.Freight,
                    ShipName = request.ShipName,
                    ShipAddress = request.ShipAddress,
                    ShipCity = request.ShipCity,
                    ShipRegion = request.ShipRegion,
                    ShipPostalCode = request.ShipPostalCode,
                    ShipCountry = request.ShipCountry
                };

                await _unitOfWork.Repository<Orders>().AddAsync(order, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken); // OrderId almak için

                var orderItems = new List<CreateOrderItemResponse>();
                foreach (var item in request.Items)
                {
                    var product = products.First(x => x.ProductId == item.ProductId);

                    await _unitOfWork.Repository<OrderDetails>().AddAsync(new OrderDetails
                    {
                        OrderId = order.OrderId,
                        ProductId = item.ProductId,
                        UnitPrice = product.UnitPrice ?? 0,
                        Quantity = item.Quantity,
                        Discount = item.Discount
                    }, cancellationToken);

                    product.UnitsInStock -= item.Quantity;
                    _unitOfWork.Repository<Products>().Update(product);

                    orderItems.Add(new CreateOrderItemResponse
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = product.UnitPrice ?? 0,
                        Discount = item.Discount,
                        LineTotal = (decimal)(item.Quantity * (product.UnitPrice ?? 0) * (decimal)(1 - item.Discount))
                    });
                }

           
                await _outboxService.AddMessageAsync(new OrderCreatedEvent
                {
                    OrderId = order.OrderId,
                    CustomerId = customer.CustomerId,
                    CompanyName = customer.CompanyName,
                    CustomerEmail = $"{customer.CustomerId.ToLower()}@example.com",
                    TotalAmount = orderItems.Sum(x => x.LineTotal),
                    OrderDate = order.OrderDate!.Value
                }, cancellationToken);

                
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return new CreateOrderCommandResponse
                {
                    OrderId = order.OrderId,
                    CustomerId = order.CustomerId,
                    OrderDate = order.OrderDate!.Value,
                    TotalAmount = orderItems.Sum(x => x.LineTotal),
                    Items = orderItems
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
        public async Task<bool> DeleteAsync(int orderId, CancellationToken cancellationToken)
        {
            await _orderBusinessRules.OrderMustExistAsync(orderId, cancellationToken);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {

                var orderDetails = await _unitOfWork.Repository<OrderDetails>()
                    .GetAll()
                    .Include(x => x.Product)
                    .Where(x => x.OrderId == orderId)
                    .ToListAsync(cancellationToken);


                foreach (var detail in orderDetails)
                {
                    if (detail.Product != null)
                    {
                        detail.Product.UnitsInStock += detail.Quantity;
                        _unitOfWork.Repository<Products>().Update(detail.Product);
                    }
                    _unitOfWork.Repository<OrderDetails>().Delete(detail);
                }


                var order = await _unitOfWork.Repository<Orders>()
                    .GetAll()
                    .FirstOrDefaultAsync(x => x.OrderId == orderId, cancellationToken);

                _unitOfWork.Repository<Orders>().Delete(order!);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }

}
