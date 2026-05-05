using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NorthwindApi.Application.Features.Reports.GetCustomerRFM;
using NorthwindApi.Application.Features.Reports.GetEmployeePerformance;
using NorthwindApi.Application.Features.Reports.GetSalesByEmployee;
using NorthwindApi.Application.Features.Reports.GetSalesByProduct;
using NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByCategory;
using NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByPeriod;
using NorthwindApi.Application.Features.Reports.GetStockAnalysis;
using NorthwindApi.Application.Features.Reports.GetTopSellingProducts;
using Swashbuckle.AspNetCore.Annotations;

/// <summary>
/// Raporlama ve analiz işlemleri
/// </summary>
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[EnableRateLimiting("ReportPolicy")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Dönemsel satış raporu getirir
    /// </summary>
    [HttpGet("GetSalesByPeriod", Name = "GetSalesByPeriod")]
    [SwaggerOperation(
        Summary = "Dönemsel satış raporu",
        Description = "Yıl/ay bazında satış toplamı, sipariş sayısı ve ortalama sipariş değeri",
        OperationId = "GetSalesByPeriod",
        Tags = new[] { "Reports" })]
    [SwaggerResponse(200, "Başarılı", typeof(List<GetSalesByPeriodResponse>))]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> GetSalesByPeriod(
        [FromQuery] GetSalesByPeriodQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Kategori bazlı satış raporu getirir
    /// </summary>
    [HttpGet("GetSalesByCategory", Name = "GetSalesByCategory")]
    [SwaggerOperation(
        Summary = "Kategori bazlı satış raporu",
        Description = "Kategori bazında satış toplamı, sipariş sayısı ve en çok satan ürün",
        OperationId = "GetSalesByCategory",
        Tags = new[] { "Reports" })]
    [SwaggerResponse(200, "Başarılı", typeof(List<GetSalesByCategoryResponse>))]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> GetSalesByCategory(
        [FromQuery] GetSalesByCategoryQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Stok analizi yapar
    /// </summary>
    [HttpGet("GetStockAnalysis", Name = "GetStockAnalysis")]
    [SwaggerOperation(
        Summary = "Stok analizi",
        Description = "Kritik stok, fazla stok, devir hızı ve discontinued ürün analizi",
        OperationId = "GetStockAnalysis",
        Tags = new[] { "Reports" })]
    [SwaggerResponse(200, "Başarılı", typeof(GetStockAnalysisResponse))]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> GetStockAnalysis(
        [FromQuery] GetStockAnalysisQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Çalışan performans raporu getirir
    /// </summary>
    [HttpGet("GetEmployeePerformance", Name = "GetEmployeePerformance")]
    [EnableRateLimiting("ReportConcurrencyPolicy")]
    [SwaggerOperation(
        Summary = "Çalışan performans raporu",
        Description = "Çalışan bazında sipariş, gelir, en çok sattığı kategori ve müşteri analizi",
        OperationId = "GetEmployeePerformance",
        Tags = new[] { "Reports" })]
    [SwaggerResponse(200, "Başarılı", typeof(List<GetEmployeePerformanceResponse>))]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> GetEmployeePerformance(
        [FromQuery] GetEmployeePerformanceQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Müşteri RFM segmentasyonu yapar
    /// </summary>
    [HttpGet("GetCustomerRFM", Name = "GetCustomerRFM")]
    [EnableRateLimiting("ReportConcurrencyPolicy")]
    [SwaggerOperation(
        Summary = "Müşteri RFM analizi",
        Description = "Recency, Frequency, Monetary skorlama ile müşteri segmentasyonu",
        OperationId = "GetCustomerRFM",
        Tags = new[] { "Reports" })]
    [SwaggerResponse(200, "Başarılı", typeof(GetCustomerRFMResult))]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> GetCustomerRFM(
        [FromQuery] GetCustomerRFMQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// En çok satan ürünler raporunu getirir
    /// </summary>
    [HttpGet("GetTopSellingProducts", Name = "GetTopSellingProducts")]
    [SwaggerOperation(
        Summary = "En çok satan ürünler",
        Description = "Önceki döneme göre trend karşılaştırmalı en çok satan ürünler",
        OperationId = "GetTopSellingProducts",
        Tags = new[] { "Reports" })]
    [SwaggerResponse(200, "Başarılı", typeof(List<GetTopSellingProductsResponse>))]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> GetTopSellingProducts(
        [FromQuery] GetTopSellingProductsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Ürün bazlı satış raporu getirir
    /// </summary>
    [HttpGet("GetSalesByProduct", Name = "GetSalesByProduct")]
    [SwaggerOperation(
        Summary = "Ürün bazlı satış raporu",
        Description = "Ürün bazında satış miktarı, gelir ve sipariş sayısı",
        OperationId = "GetSalesByProduct",
        Tags = new[] { "Reports" })]
    [SwaggerResponse(200, "Başarılı", typeof(List<GetSalesByProductResponse>))]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> GetSalesByProduct(
        [FromQuery] GetSalesByProductQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Çalışan bazlı satış raporu getirir
    /// </summary>
    [HttpGet("GetSalesByEmployee", Name = "GetSalesByEmployee")]
    [SwaggerOperation(
        Summary = "Çalışan bazlı satış raporu",
        Description = "Çalışan bazında satış toplamı, gelir ve en çok sattığı kategori/müşteri",
        OperationId = "GetSalesByEmployee",
        Tags = new[] { "Reports" })]
    [SwaggerResponse(200, "Başarılı", typeof(List<GetSalesByEmployeeResponse>))]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> GetSalesByEmployee(
        [FromQuery] GetSalesByEmployeeQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}