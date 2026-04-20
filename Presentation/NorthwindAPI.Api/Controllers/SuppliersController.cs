using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Application.Features.Suppliers.Commands.CreateSupplier;
using NorthwindApi.Application.Features.Suppliers.Commands.DeleteSupplier;
using NorthwindApi.Application.Features.Suppliers.Commands.UpdateSupplier;
using NorthwindApi.Application.Features.Suppliers.Queries.GetSuppliers;

namespace NorthwindAPI.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SuppliersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetSuppliers", Name = "GetSuppliers")]
        public async Task<IActionResult> GetSuppliers(
            [FromQuery] GetSuppliersQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPost("CreateSupplier", Name = "CreateSupplier")]
        public async Task<IActionResult> CreateSupplier(
            [FromBody] CreateSupplierCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetSuppliers), new { id = result.SupplierId }, result);
        }

        [HttpPut("UpdateSupplier", Name = "UpdateSupplier")]
        public async Task<IActionResult> UpdateSupplier(
            [FromBody] UpdateSupplierCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("DeleteSupplier/{supplierId}", Name = "DeleteSupplier")]
        public async Task<IActionResult> DeleteSupplier(
            int supplierId,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteSupplierCommand { SupplierId = supplierId }, cancellationToken);
            return NoContent();
        }
    }
}