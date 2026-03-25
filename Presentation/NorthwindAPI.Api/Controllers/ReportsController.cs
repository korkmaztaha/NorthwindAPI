using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Application.Features.Reports.GetCustomerRFM;
using NorthwindApi.Application.Features.Reports.GetEmployeePerformance;
using NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByCategory;
using NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByPeriod;
using NorthwindApi.Application.Features.Reports.GetStockAnalysis;
using NorthwindApi.Application.Features.Reports.GetTopSellingProducts;

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

        [HttpGet("GetEmployeePerformance", Name = "GetEmployeePerformance")]
        public async Task<IActionResult> GetEmployeePerformance(
            [FromQuery] GetEmployeePerformanceQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }


        [HttpGet("GetCustomerRFM", Name = "GetCustomerRFM")]
        public async Task<IActionResult> GetCustomerRFM(
            [FromQuery] GetCustomerRFMQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("GetTopSellingProducts", Name = "GetTopSellingProducts")]
        public async Task<IActionResult> GetTopSellingProducts(
            [FromQuery] GetTopSellingProductsQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
