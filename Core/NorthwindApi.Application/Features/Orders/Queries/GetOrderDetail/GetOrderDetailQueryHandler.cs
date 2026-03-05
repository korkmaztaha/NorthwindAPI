using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Orders.Queries.GetOrderDetail
{
    public  class GetOrderDetailQueryHandler : IRequestHandler<GetOrderDetailQuery, GetOrderDetailResponse>
    {
        private readonly IOrderService _orderService;

        public GetOrderDetailQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<GetOrderDetailResponse> Handle(
            GetOrderDetailQuery request,
            CancellationToken cancellationToken)
            => await _orderService.GetDetailAsync(request.OrderId, cancellationToken);
    }
}
