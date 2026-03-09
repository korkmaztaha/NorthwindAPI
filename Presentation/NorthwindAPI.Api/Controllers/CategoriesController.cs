using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Application.Features.Categories.Commands.CreateCategory;
using NorthwindApi.Application.Features.Categories.Commands.DeleteCategory;
using NorthwindApi.Application.Features.Categories.Commands.UpdateCategory;
using NorthwindApi.Application.Features.Categories.Queries.GetCategories;
using NorthwindApi.Application.Features.Categories.Queries.GetCategoryDetail;

namespace NorthwindAPI.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetCategories", Name = "GetCategories")]
        public async Task<IActionResult> GetCategories(
            [FromQuery] GetCategoriesQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("GetCategoryDetail/{categoryId}", Name = "GetCategoryDetail")]
        public async Task<IActionResult> GetCategoryDetail(
            int categoryId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new GetCategoryDetailQuery { CategoryId = categoryId },
                cancellationToken);
            return Ok(result);
        }

        [HttpPost("CreateCategory", Name = "CreateCategory")]
        public async Task<IActionResult> CreateCategory(
            [FromBody] CreateCategoryCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetCategories), new { id = result.CategoryId }, result);
        }

        [HttpPut("UpdateCategory", Name = "UpdateCategory")]
        public async Task<IActionResult> UpdateCategory(
            [FromBody] UpdateCategoryCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("DeleteCategory/{categoryId}", Name = "DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(
            int categoryId,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteCategoryCommand { CategoryId = categoryId }, cancellationToken);
            return NoContent();
        }
    }
}
