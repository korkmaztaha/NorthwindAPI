using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Features.Products.Commands.CreateProduct;
using NorthwindApi.Application.Features.Products.Commands.UpdateProduct;
using NorthwindApi.Application.Features.Products.Queries.GetProducts;
using NorthwindApi.Application.Interfaces.BusinessRules;
using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Application.Interfaces.Services;
using NorthwindApi.Domain.Entities;

namespace NorthwindApi.Persistence.Services.EntityServices
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductBusinessRules _productBusinessRules;

        public ProductService(
            IUnitOfWork unitOfWork,
            IProductBusinessRules productBusinessRules)
        {
            _unitOfWork = unitOfWork;
            _productBusinessRules = productBusinessRules;
        }

        public async Task<List<GetProductsQueryResponse>> GetAllAsync(
            GetProductsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Products>()
                .GetAll()
                .AsNoTracking();

            if (!string.IsNullOrEmpty(request.ProductName))
                query = query.Where(x => x.ProductName.Contains(request.ProductName));

            if (request.ProductId.HasValue)
                query = query.Where(x => x.ProductId == request.ProductId);

            if (request.CategoryId.HasValue)
                query = query.Where(x => x.CategoryId == request.CategoryId);

            if (request.SupplierId.HasValue)
                query = query.Where(x => x.SupplierId == request.SupplierId);

            if (request.MinPrice.HasValue)
                query = query.Where(x => x.UnitPrice >= request.MinPrice);

            if (request.MaxPrice.HasValue)
                query = query.Where(x => x.UnitPrice <= request.MaxPrice);

            if (request.Discontinued.HasValue)
                query = query.Where(x => x.Discontinued == request.Discontinued);

            return await query
                .OrderBy(x => x.ProductName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new GetProductsQueryResponse
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    CategoryName = x.Category != null ? x.Category.CategoryName : null,
                    SupplierName = x.Supplier != null ? x.Supplier.CompanyName : null,
                    QuantityPerUnit = x.QuantityPerUnit,
                    UnitPrice = x.UnitPrice,
                    UnitsInStock = x.UnitsInStock,
                    UnitsOnOrder = x.UnitsOnOrder,
                    ReorderLevel = x.ReorderLevel,
                    Discontinued = x.Discontinued
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<CreateProductCommandResponse> CreateAsync(
            CreateProductCommand request,
            CancellationToken cancellationToken)
        {
            
            await _productBusinessRules.ProductNameMustBeUniqueAsync(
                request.ProductName,
                cancellationToken);

            var product = new Products
            {
                ProductName = request.ProductName,
                SupplierId = request.SupplierId,
                CategoryId = request.CategoryId,
                QuantityPerUnit = request.QuantityPerUnit,
                UnitPrice = request.UnitPrice,
                UnitsInStock = request.UnitsInStock,
                UnitsOnOrder = request.UnitsOnOrder,
                ReorderLevel = request.ReorderLevel,
                Discontinued = request.Discontinued
            };

            await _unitOfWork.Repository<Products>()
                .AddAsync(product, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateProductCommandResponse
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CreatedAt = DateTime.UtcNow
            };
        }

        public async Task<UpdateProductCommandResponse> UpdateAsync(
            UpdateProductCommand request,
            CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Repository<Products>()
                .GetAll()
                .FirstOrDefaultAsync(x => x.ProductId == request.ProductId, cancellationToken);

            if (product is null)
                throw new KeyNotFoundException($"{request.ProductId} ID'li ürün bulunamadı.");

        
            await _productBusinessRules.ProductNameMustBeUniqueForUpdateAsync(
                request.ProductId,
                request.ProductName,
                cancellationToken);

            product.ProductName = request.ProductName;
            product.SupplierId = request.SupplierId;
            product.CategoryId = request.CategoryId;
            product.QuantityPerUnit = request.QuantityPerUnit;
            product.UnitPrice = request.UnitPrice;
            product.UnitsInStock = request.UnitsInStock;
            product.UnitsOnOrder = request.UnitsOnOrder;
            product.ReorderLevel = request.ReorderLevel;
            product.Discontinued = request.Discontinued;

            _unitOfWork.Repository<Products>().Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateProductCommandResponse
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<bool> DeleteAsync(
            int productId,
            CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Repository<Products>()
                .GetAll()
                .FirstOrDefaultAsync(x => x.ProductId == productId, cancellationToken);

            if (product is null)
                throw new KeyNotFoundException($"{productId} ID'li ürün bulunamadı.");

            _unitOfWork.Repository<Products>().Delete(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}