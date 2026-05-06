using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Basket.Commands.RemoveFromBasket
{
    public class RemoveFromBasketCommand : IRequest
    {
        public string CustomerId { get; set; } = null!;
        public int ProductId { get; set; }
    }
}
