using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Features.Orders.Commands.CreateOrder;
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
    public class ProductBusinessRules : IProductBusinessRules
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductBusinessRules(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

      

        public async Task ProductsMustExistAsync(
            List<int> productIds,
            CancellationToken cancellationToken)
        {
            var foundIds = await _unitOfWork.Repository<Products>()
                .GetAll()
                .Where(x => productIds.Contains(x.ProductId))
                .Select(x => x.ProductId)
                .ToListAsync(cancellationToken);

            var missingIds = productIds.Except(foundIds).ToList();

            if (missingIds.Any())
                throw new KeyNotFoundException(
                    $"Ürün/ürünler bulunamadı: {string.Join(", ", missingIds)}");
        }

        public async Task<List<Products>> GetAndValidateProductsAsync(
            List<CreateOrderItemCommand> items,
            CancellationToken cancellationToken)
        {
            var productIds = items.Select(x => x.ProductId).ToList();

            var products = await _unitOfWork.Repository<Products>()
                .GetAll()
                .Where(x => productIds.Contains(x.ProductId))
                .ToListAsync(cancellationToken);

            var missingIds = productIds.Except(products.Select(x => x.ProductId)).ToList();
            if (missingIds.Any())
                throw new KeyNotFoundException(
                    $"Şu ürünler bulunamadı: {string.Join(", ", missingIds)}");

            await StockMustBeSufficientAsync(items, cancellationToken, products);

            return products;
        }

        public async Task StockMustBeSufficientAsync(
            List<CreateOrderItemCommand> items,
            CancellationToken cancellationToken)
        {
            var productIds = items.Select(x => x.ProductId).ToList();
            var products = await _unitOfWork.Repository<Products>()
                .GetAll()
                .Where(x => productIds.Contains(x.ProductId))
                .ToListAsync(cancellationToken);

            await StockMustBeSufficientAsync(items, cancellationToken, products);
        }

        public async Task ProductNameMustBeUniqueAsync(
            string productName,
            CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Repository<Products>()
                .GetAll()
                .AnyAsync(x => x.ProductName == productName, cancellationToken);

            if (exists)
                throw new InvalidOperationException($"{productName} adlı ürün zaten mevcut.");
        }

        private Task StockMustBeSufficientAsync(
            List<CreateOrderItemCommand> items,
            CancellationToken cancellationToken,
            List<Products> products)
        {
            foreach (var item in items)
            {
                var product = products.First(x => x.ProductId == item.ProductId);
                if (product.UnitsInStock < item.Quantity)
                    throw new InvalidOperationException(
                        $"{product.ProductName} için yeterli stok yok. Mevcut: {product.UnitsInStock}, İstenen: {item.Quantity}");
            }

            return Task.CompletedTask;
        }
    }
}
