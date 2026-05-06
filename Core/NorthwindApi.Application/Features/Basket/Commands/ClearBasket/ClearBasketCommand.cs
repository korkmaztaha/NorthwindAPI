using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Basket.Commands.ClearBasket
{
    public class ClearBasketCommand : IRequest
    {
        public string CustomerId { get; set; } = null!;
    }
}
