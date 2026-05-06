using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Basket.Commands.AddToBasket
{
    public class AddToBasketCommandHandler : IRequestHandler<AddToBasketCommand, AddToBasketCommandResponse>
    {
        private readonly IBasketService _basketService;

        public AddToBasketCommandHandler(IBasketService basketService)
        {
            _basketService = basketService;
        }

        public async Task<AddToBasketCommandResponse> Handle(
            AddToBasketCommand request,
            CancellationToken cancellationToken)
            => await _basketService.AddToBasketAsync(request, cancellationToken);
    }
}
