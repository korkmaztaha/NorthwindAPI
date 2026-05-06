using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Basket.Commands.AddToBasket
{
    public class AddToBasketCommand : IRequest<AddToBasketCommandResponse>
    {
        public string CustomerId { get; set; } = null!;
        public int ProductId { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
    }
}
