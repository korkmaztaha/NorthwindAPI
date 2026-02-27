using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Application.Features.Products.Commands.CreateProduct;
using NorthwindApi.Application.Features.Products.Commands.DeleteProduct;
using NorthwindApi.Application.Features.Products.Commands.UpdateProduct;
using NorthwindApi.Application.Features.Products.Queries.GetProducts;

namespace NorthwindAPI.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetProducts", Name = "GetProducts")]
        public async Task<IActionResult> GetProducts(
            [FromQuery] GetProductsQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPost("CreateProduct", Name = "CreateProduct")]
        public async Task<IActionResult> CreateProduct(
            [FromBody] CreateProductCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetProducts), new { id = result.ProductId }, result);
        }

        [HttpPut("UpdateProduct", Name = "UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(
            [FromBody] UpdateProductCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("DeleteProduct/{productId}", Name = "DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(
            int productId,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteProductCommand { ProductId = productId }, cancellationToken);
            return NoContent();
        }
    }
}
