using NorthwindApi.Application.Features.Products.Commands.CreateProduct;
using NorthwindApi.Application.Features.Products.Commands.UpdateProduct;
using NorthwindApi.Application.Features.Products.Queries.GetProducts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<List<GetProductsQueryResponse>> GetAllAsync(GetProductsQuery request, CancellationToken cancellationToken);
        Task<CreateProductCommandResponse> CreateAsync(CreateProductCommand request, CancellationToken cancellationToken);
        Task<UpdateProductCommandResponse> UpdateAsync(UpdateProductCommand request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int productId, CancellationToken cancellationToken);
    }
}
