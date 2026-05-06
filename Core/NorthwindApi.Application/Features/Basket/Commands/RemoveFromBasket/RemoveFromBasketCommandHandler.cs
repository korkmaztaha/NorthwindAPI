using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Basket.Commands.RemoveFromBasket
{
    public class RemoveFromBasketCommandHandler : IRequestHandler<RemoveFromBasketCommand>
    {
        private readonly IBasketService _basketService;

        public RemoveFromBasketCommandHandler(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task Handle(
            RemoveFromBasketCommand request,
            CancellationToken cancellationToken)
            => await _basketService.RemoveFromBasketAsync(request, cancellationToken);
    }
}
