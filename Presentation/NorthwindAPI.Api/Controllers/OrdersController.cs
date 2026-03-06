using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Application.Features.Orders.Commands.CreateOrder;
using NorthwindApi.Application.Features.Orders.Commands.DeleteOrder;
using NorthwindApi.Application.Features.Orders.Queries.GetOrderDetail;
using NorthwindApi.Application.Features.Orders.Queries.GetOrders;

namespace NorthwindAPI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetOrders", Name = "GetOrders")]
        public async Task<IActionResult> GetOrders(
        [FromQuery] GetOrdersQuery query,
        CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("GetOrderDetail/{orderId}", Name = "GetOrderDetail")]
        public async Task<IActionResult> GetOrderDetail(
            int orderId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetOrderDetailQuery { OrderId = orderId },
                cancellationToken);
            return Ok(result);

        }
        [HttpPost("CreateOrder", Name = "CreateOrder")]
        public async Task<IActionResult> CreateOrder(
             [FromBody] CreateOrderCommand command,
             CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetOrderDetail), new { orderId = result.OrderId }, result);
        }

        [HttpDelete("DeleteOrder/{orderId}", Name = "DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(
            int orderId,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteOrderCommand { OrderId = orderId }, cancellationToken);
            return Ok();
        }
    }
}
