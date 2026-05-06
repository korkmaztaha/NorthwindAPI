using NorthwindApi.Application.Features.Basket.Commands.AddToBasket;
using NorthwindApi.Application.Features.Basket.Commands.ClearBasket;
using NorthwindApi.Application.Features.Basket.Commands.RemoveFromBasket;
using NorthwindApi.Application.Features.Basket.Queries.GetBasket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.Services
{
    public interface IBasketService
    {
        Task<GetBasketQueryResponse?> GetBasketAsync(GetBasketQuery request, CancellationToken cancellationToken);
        Task<AddToBasketCommandResponse> AddToBasketAsync(AddToBasketCommand request, CancellationToken cancellationToken);
        Task RemoveFromBasketAsync(RemoveFromBasketCommand request, CancellationToken cancellationToken);
        Task ClearBasketAsync(ClearBasketCommand request, CancellationToken cancellationToken);
    }
}
