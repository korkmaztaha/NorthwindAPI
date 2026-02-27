using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Products.Queries.GetProducts
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<GetProductsQueryResponse>>
    {
        private readonly IProductService _productService;

        public GetProductsQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<List<GetProductsQueryResponse>> Handle(
            GetProductsQuery request,
            CancellationToken cancellationToken)
            => await _productService.GetAllAsync(request, cancellationToken);
    }
}
