using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

using NorthwindApi.Application.Features.Basket.Commands.AddToBasket;
using NorthwindApi.Application.Features.Basket.Commands.ClearBasket;
using NorthwindApi.Application.Features.Basket.Commands.RemoveFromBasket;
using NorthwindApi.Application.Features.Basket.Queries.GetBasket;
using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Application.Interfaces.Services;
using NorthwindApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Persistence.Services.EntityServices
{
    public class BasketService : IBasketService
    {
        private readonly IMongoCollection<Basket> _basketCollection;
        private readonly IUnitOfWork _unitOfWork;

        public BasketService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            var client = new MongoClient(configuration["MongoDB:ConnectionString"]);
            var database = client.GetDatabase(configuration["MongoDB:DatabaseName"]);
            _basketCollection = database.GetCollection<Basket>("Baskets");
        }

        public async Task<GetBasketQueryResponse?> GetBasketAsync(
            GetBasketQuery request,
            CancellationToken cancellationToken)
        {
            var basket = await _basketCollection
                .Find(b => b.CustomerId == request.CustomerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (basket == null) return null;

            return new GetBasketQueryResponse
            {
                CustomerId = basket.CustomerId,
                CompanyName = basket.CompanyName,
                TotalAmount = basket.TotalAmount,
                UpdatedAt = basket.UpdatedAt,
                Items = basket.Items.Select(i => new BasketItemResponse
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    Discount = i.Discount,
                    LineTotal = i.LineTotal
                }).ToList()
            };
        }

        public async Task<AddToBasketCommandResponse> AddToBasketAsync(
            AddToBasketCommand request,
            CancellationToken cancellationToken)
        {
            // Müşteri ve ürün bilgilerini SQL Server'dan çek
            var customer = await _unitOfWork.Repository<Customer>()
                .GetAll()
                .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId, cancellationToken)
                ?? throw new KeyNotFoundException($"Customer {request.CustomerId} not found.");

            var product = await _unitOfWork.Repository<Products>()
                .GetAll()
                .FirstOrDefaultAsync(p => p.ProductId == request.ProductId, cancellationToken)
                ?? throw new KeyNotFoundException($"Product {request.ProductId} not found.");

            // Mevcut sepeti getir veya yeni oluştur
            var basket = await _basketCollection
                .Find(b => b.CustomerId == request.CustomerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (basket == null)
            {
                basket = new Basket
                {
                    CustomerId = request.CustomerId,
                    CompanyName = customer.CompanyName,
                    Items = new List<BasketItem>()
                };
            }

            // Ürün zaten sepette var mı?
            var existingItem = basket.Items.FirstOrDefault(i => i.ProductId == request.ProductId);

            if (existingItem != null)
            {
                // Miktarı güncelle
                existingItem.Quantity += request.Quantity;
                existingItem.Discount = request.Discount;
            }
            else
            {
                // Yeni ürün ekle
                basket.Items.Add(new BasketItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    UnitPrice = product.UnitPrice ?? 0,
                    Quantity = request.Quantity,
                    Discount = request.Discount
                });
            }

            basket.UpdatedAt = DateTime.UtcNow;

            // MongoDB'ye kaydet
            await _basketCollection.ReplaceOneAsync(
                b => b.CustomerId == request.CustomerId,
                basket,
                new ReplaceOptions { IsUpsert = true },
                cancellationToken);

            return new AddToBasketCommandResponse
            {
                CustomerId = basket.CustomerId,
                TotalAmount = basket.TotalAmount,
                TotalItems = basket.Items.Sum(i => i.Quantity)
            };
        }

        public async Task RemoveFromBasketAsync(
            RemoveFromBasketCommand request,
            CancellationToken cancellationToken)
        {
            var basket = await _basketCollection
                .Find(b => b.CustomerId == request.CustomerId)
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new KeyNotFoundException($"Basket not found for customer {request.CustomerId}.");

            var item = basket.Items.FirstOrDefault(i => i.ProductId == request.ProductId)
                ?? throw new KeyNotFoundException($"Product {request.ProductId} not found in basket.");

            basket.Items.Remove(item);
            basket.UpdatedAt = DateTime.UtcNow;

            await _basketCollection.ReplaceOneAsync(
                b => b.CustomerId == request.CustomerId,
                basket,
                cancellationToken: cancellationToken);
        }

        public async Task ClearBasketAsync(
            ClearBasketCommand request,
            CancellationToken cancellationToken)
        {
            var result = await _basketCollection.DeleteOneAsync(
                b => b.CustomerId == request.CustomerId,
                cancellationToken);

            if (result.DeletedCount == 0)
                throw new KeyNotFoundException($"Basket not found for customer {request.CustomerId}.");
        }
    }
}
