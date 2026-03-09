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

    public class CategoryBusinessRules : ICategoryBusinessRules
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryBusinessRules(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CategoryMustExistAsync(int categoryId, CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Repository<Categories>()
                .GetAll()
                .AnyAsync(x => x.CategoryId == categoryId, cancellationToken);

            if (!exists)
                throw new KeyNotFoundException($"{categoryId} ID'li kategori bulunamadı.");
        }

        public async Task CategoryNameMustBeUniqueAsync(
            string categoryName,
            CancellationToken cancellationToken)
        {
            var exists = await _unitOfWork.Repository<Categories>()
                .GetAll()
                .AnyAsync(x => x.CategoryName == categoryName, cancellationToken);

            if (exists)
                throw new InvalidOperationException($"{categoryName} adlı kategori zaten mevcut.");
        }

        public async Task CategoryHasNoProductsAsync(int categoryId, CancellationToken cancellationToken)
        {
            var hasProducts = await _unitOfWork.Repository<Products>()
                .GetAll()
                .AnyAsync(x => x.CategoryId == categoryId, cancellationToken);

            if (hasProducts)
                throw new InvalidOperationException(
                    $"{categoryId} ID'li kategoriye ait ürünler var, silinemez.");
        }
    }
}
