using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderCommandResponse>
    {
        private readonly IOrderService _orderService;

        public CreateOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<CreateOrderCommandResponse> Handle(
            CreateOrderCommand request,
            CancellationToken cancellationToken)
            => await _orderService.CreateAsync(request, cancellationToken);
    }
}
