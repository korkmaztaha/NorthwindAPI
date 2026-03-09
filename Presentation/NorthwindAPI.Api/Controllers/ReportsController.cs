using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByCategory;
using NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByPeriod;
using NorthwindApi.Application.Features.Reports.GetStockAnalysis;

namespace NorthwindAPI.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetSalesByPeriod", Name = "GetSalesByPeriod")]
        public async Task<IActionResult> GetSalesByPeriod(
            [FromQuery] GetSalesByPeriodQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        [HttpGet("GetSalesByCategory", Name = "GetSalesByCategory")]
        public async Task<IActionResult> GetSalesByCategory(
             [FromQuery] GetSalesByCategoryQuery query,
             CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        [HttpGet("GetStockAnalysis", Name = "GetStockAnalysis")]
        public async Task<IActionResult> GetStockAnalysis(
              [FromQuery] GetStockAnalysisQuery query,
              CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
