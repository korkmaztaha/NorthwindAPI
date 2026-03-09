using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Categories.Queries.GetCategories
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<GetCategoriesQueryResponse>>
    {
        private readonly ICategoryService _categoryService;

        public GetCategoriesQueryHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<List<GetCategoriesQueryResponse>> Handle(
            GetCategoriesQuery request,
            CancellationToken cancellationToken)
            => await _categoryService.GetAllAsync(request, cancellationToken);
    }
}
