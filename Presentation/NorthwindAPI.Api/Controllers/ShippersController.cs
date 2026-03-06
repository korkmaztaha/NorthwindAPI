using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Application.Features.Shippers.Commands.CreateShipper;
using NorthwindApi.Application.Features.Shippers.Commands.DeleteShipper;
using NorthwindApi.Application.Features.Shippers.Commands.UpdateShipper;
using NorthwindApi.Application.Features.Shippers.Queries.GetShippers;

namespace NorthwindAPI.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ShippersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShippersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetShippers", Name = "GetShippers")]
        public async Task<IActionResult> GetShippers(
            [FromQuery] GetShippersQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPost("CreateShipper", Name = "CreateShipper")]
        public async Task<IActionResult> CreateShipper(
            [FromBody] CreateShipperCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetShippers), new { id = result.ShipperId }, result);
        }

        [HttpPut("UpdateShipper", Name = "UpdateShipper")]
        public async Task<IActionResult> UpdateShipper(
            [FromBody] UpdateShipperCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("DeleteShipper/{shipperId}", Name = "DeleteShipper")]
        public async Task<IActionResult> DeleteShipper(
            int shipperId,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteShipperCommand { ShipperId = shipperId }, cancellationToken);
            return NoContent();
        }
    }
}
