using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Basket.Queries.GetBasket
{
    public class GetBasketQuery : IRequest<GetBasketQueryResponse?>
    {
        public string CustomerId { get; set; } = null!;
    }
}
