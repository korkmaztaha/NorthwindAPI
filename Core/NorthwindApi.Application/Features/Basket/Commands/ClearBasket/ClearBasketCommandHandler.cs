using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Basket.Commands.ClearBasket
{
    public class ClearBasketCommandHandler : IRequestHandler<ClearBasketCommand>
    {
        private readonly IBasketService _basketService;

        public ClearBasketCommandHandler(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task Handle(
            ClearBasketCommand request,
            CancellationToken cancellationToken)
            => await _basketService.ClearBasketAsync(request, cancellationToken);
    }
}
