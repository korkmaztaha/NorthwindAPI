using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Categories.Queries.GetCategoryDetail
{
    public class GetCategoryDetailQueryHandler : IRequestHandler<GetCategoryDetailQuery, GetCategoryDetailQueryResponse>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoryDetailQueryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<GetCategoryDetailQueryResponse> Handle(
            GetCategoryDetailQuery request,
            CancellationToken cancellationToken)
            => await _categoryService.GetDetailAsync(request.CategoryId, cancellationToken);
    }
}
