using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Basket.Queries.GetBasket
{
    public class GetBasketQueryHandler : IRequestHandler<GetBasketQuery, GetBasketQueryResponse?>
    {
        private readonly IBasketService _basketService;

        public GetBasketQueryHandler(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task<GetBasketQueryResponse?> Handle(
            GetBasketQuery request,
            CancellationToken cancellationToken)
            => await _basketService.GetBasketAsync(request, cancellationToken);
    }
}
