using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Features.Categories.Commands.CreateCategory;
using NorthwindApi.Application.Features.Categories.Commands.UpdateCategory;
using NorthwindApi.Application.Features.Categories.Queries.GetCategories;
using NorthwindApi.Application.Features.Categories.Queries.GetCategoryDetail;
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
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryBusinessRules _businessRules;

        public CategoryService(IUnitOfWork unitOfWork, ICategoryBusinessRules businessRules)
        {
            _unitOfWork = unitOfWork;
            _businessRules = businessRules;
        }

        public async Task<List<GetCategoriesQueryResponse>> GetAllAsync(
            GetCategoriesQuery request,
            CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Categories>().GetAll();

            if (!string.IsNullOrEmpty(request.CategoryName))
                query = query.Where(x => x.CategoryName.Contains(request.CategoryName));

            return await query
                .OrderBy(x => x.CategoryName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new GetCategoriesQueryResponse
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    Description = x.Description,
                    TotalProducts = x.Products.Count
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<GetCategoryDetailQueryResponse> GetDetailAsync(
            int categoryId,
            CancellationToken cancellationToken)
        {
            await _businessRules.CategoryMustExistAsync(categoryId, cancellationToken);

            var category = await _unitOfWork.Repository<Categories>()
                .GetAll()
                .Where(x => x.CategoryId == categoryId)
                .Select(x => new GetCategoryDetailQueryResponse
                {
                    CategoryId = x.CategoryId,
                    CategoryName = x.CategoryName,
                    Description = x.Description,
                    TotalProducts = x.Products.Count,
                    Products = x.Products.Select(p => new CategoryProductResponse
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        UnitPrice = p.UnitPrice,
                        UnitsInStock = p.UnitsInStock,
                        Discontinued = p.Discontinued
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            return category!;
        }

        public async Task<CreateCategoryCommandResponse> CreateAsync(
            CreateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            await _businessRules.CategoryNameMustBeUniqueAsync(request.CategoryName, cancellationToken);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
               
                var category = new Categories
                {
                    CategoryName = request.CategoryName,
                    Description = request.Description
                };

                await _unitOfWork.Repository<Categories>().AddAsync(category, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                
                var productResponses = new List<CreateCategoryProductResponse>();

                foreach (var item in request.Products)
                {
                    var product = new Products
                    {
                        ProductName = item.ProductName,
                        CategoryId = category.CategoryId,
                        SupplierId = item.SupplierId,
                        QuantityPerUnit = item.QuantityPerUnit,
                        UnitPrice = item.UnitPrice,
                        UnitsInStock = item.UnitsInStock,
                        UnitsOnOrder = item.UnitsOnOrder,
                        ReorderLevel = item.ReorderLevel,
                        Discontinued = item.Discontinued
                    };

                    await _unitOfWork.Repository<Products>().AddAsync(product, cancellationToken);

                    productResponses.Add(new CreateCategoryProductResponse
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        UnitPrice = product.UnitPrice
                    });
                }

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return new CreateCategoryCommandResponse
                {
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    CreatedAt = DateTime.UtcNow,
                    Products = productResponses
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        public async Task<UpdateCategoryCommandResponse> UpdateAsync(
            UpdateCategoryCommand request,
            CancellationToken cancellationToken)
        {
            await _businessRules.CategoryMustExistAsync(request.CategoryId, cancellationToken);

            var category = await _unitOfWork.Repository<Categories>()
                .GetAll()
                .FirstOrDefaultAsync(x => x.CategoryId == request.CategoryId, cancellationToken);

            category!.CategoryName = request.CategoryName;
            category.Description = request.Description;

            _unitOfWork.Repository<Categories>().Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateCategoryCommandResponse
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<bool> DeleteAsync(int categoryId, CancellationToken cancellationToken)
        {
            await _businessRules.CategoryMustExistAsync(categoryId, cancellationToken);
            await _businessRules.CategoryHasNoProductsAsync(categoryId, cancellationToken);

            var category = await _unitOfWork.Repository<Categories>()
                .GetAll()
                .FirstOrDefaultAsync(x => x.CategoryId == categoryId, cancellationToken);

            _unitOfWork.Repository<Categories>().Delete(category!);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
