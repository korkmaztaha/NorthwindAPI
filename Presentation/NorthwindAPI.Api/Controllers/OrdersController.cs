using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NorthwindApi.Application.Features.Orders.Commands.CreateOrder;
using NorthwindApi.Application.Features.Orders.Commands.DeleteOrder;
using NorthwindApi.Application.Features.Orders.Queries.GetOrderDetail;
using NorthwindApi.Application.Features.Orders.Queries.GetOrders;
using Swashbuckle.AspNetCore.Annotations;

/// <summary>
/// Sipariş yönetimi işlemleri
/// </summary>
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[EnableRateLimiting("GeneralPolicy")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Sipariş listesini getirir
    /// </summary>
    [HttpGet("GetOrders", Name = "GetOrders")]
    [SwaggerOperation(
        Summary = "Sipariş listesi",
        Description = "Filtreleme ve sayfalama ile sipariş listesi getirir",
        OperationId = "GetOrders",
        Tags = new[] { "Orders" })]
    [SwaggerResponse(200, "Başarılı", typeof(List<GetOrdersQueryResponse>))]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> GetOrders(
        [FromQuery] GetOrdersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Sipariş detayını getirir
    /// </summary>
    [HttpGet("GetOrderDetail/{orderId}", Name = "GetOrderDetail")]
    [SwaggerOperation(
        Summary = "Sipariş detayı",
        Description = "Sipariş detayını ve order item'larını getirir",
        OperationId = "GetOrderDetail",
        Tags = new[] { "Orders" })]
    [SwaggerResponse(200, "Başarılı")]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(404, "Sipariş bulunamadı")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> GetOrderDetail(
        int orderId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetOrderDetailQuery { OrderId = orderId },
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Yeni sipariş oluşturur
    /// </summary>
    [HttpPost("CreateOrder", Name = "CreateOrder")]
    [SwaggerOperation(
        Summary = "Sipariş oluştur",
        Description = "Yeni sipariş oluşturur, stok günceller ve transaction kullanır",
        OperationId = "CreateOrder",
        Tags = new[] { "Orders" })]
    [SwaggerResponse(201, "Sipariş oluşturuldu")]
    [SwaggerResponse(400, "Geçersiz istek, yetersiz stok veya validation hatası")]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetOrders), new { id = result.OrderId }, result);
    }

    /// <summary>
    /// Siparişi siler ve stoku geri yükler
    /// </summary>
    [HttpDelete("DeleteOrder/{orderId}", Name = "DeleteOrder")]
    [SwaggerOperation(
        Summary = "Sipariş sil",
        Description = "Siparişi siler ve ürün stoklarını geri yükler",
        OperationId = "DeleteOrder",
        Tags = new[] { "Orders" })]
    [SwaggerResponse(204, "Sipariş silindi")]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(404, "Sipariş bulunamadı")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> DeleteOrder(
        int orderId,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteOrderCommand { OrderId = orderId }, cancellationToken);
        return NoContent();
    }
}