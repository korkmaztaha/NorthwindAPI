using NorthwindApi.Application.Features.Categories.Commands.CreateCategory;
using NorthwindApi.Application.Features.Categories.Commands.UpdateCategory;
using NorthwindApi.Application.Features.Categories.Queries.GetCategories;
using NorthwindApi.Application.Features.Categories.Queries.GetCategoryDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<List<GetCategoriesQueryResponse>> GetAllAsync(GetCategoriesQuery request, CancellationToken cancellationToken);
        Task<GetCategoryDetailQueryResponse> GetDetailAsync(int categoryId, CancellationToken cancellationToken);
        Task<CreateCategoryCommandResponse> CreateAsync(CreateCategoryCommand request, CancellationToken cancellationToken);
        Task<UpdateCategoryCommandResponse> UpdateAsync(UpdateCategoryCommand request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int categoryId, CancellationToken cancellationToken);
    }
}
