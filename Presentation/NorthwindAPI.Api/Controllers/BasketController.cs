using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NorthwindApi.Application.Features.Basket.Commands.AddToBasket;
using NorthwindApi.Application.Features.Basket.Commands.ClearBasket;
using NorthwindApi.Application.Features.Basket.Commands.RemoveFromBasket;
using NorthwindApi.Application.Features.Basket.Queries.GetBasket;
using Swashbuckle.AspNetCore.Annotations;

namespace NorthwindAPI.Api.Controllers;

/// <summary>
/// Alışveriş sepeti işlemleri (MongoDB)
/// </summary>
//[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[EnableRateLimiting("GeneralPolicy")]
public class BasketController : ControllerBase
{
    private readonly IMediator _mediator;

    public BasketController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Müşterinin sepetini getirir
    /// </summary>
    [HttpGet("{customerId}", Name = "GetBasket")]
    [SwaggerOperation(
        Summary = "Sepeti getir",
        Description = "Müşterinin MongoDB'deki sepetini getirir",
        OperationId = "GetBasket",
        Tags = new[] { "Basket" })]
    [SwaggerResponse(200, "Başarılı", typeof(GetBasketQueryResponse))]
    [SwaggerResponse(404, "Sepet bulunamadı")]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    public async Task<IActionResult> GetBasket(
        string customerId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetBasketQuery { CustomerId = customerId },
            cancellationToken);

        if (result == null)
            return NotFound($"Customer {customerId} has no basket.");

        return Ok(result);
    }

    /// <summary>
    /// Sepete ürün ekler
    /// </summary>
    [HttpPost("AddToBasket", Name = "AddToBasket")]
    [SwaggerOperation(
        Summary = "Sepete ürün ekle",
        Description = "Müşterinin sepetine ürün ekler, ürün zaten varsa miktarını günceller",
        OperationId = "AddToBasket",
        Tags = new[] { "Basket" })]
    [SwaggerResponse(200, "Ürün sepete eklendi", typeof(AddToBasketCommandResponse))]
    [SwaggerResponse(400, "Geçersiz istek")]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(404, "Müşteri veya ürün bulunamadı")]
    public async Task<IActionResult> AddToBasket(
        [FromBody] AddToBasketCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Sepetten ürün çıkarır
    /// </summary>
    [HttpDelete("{customerId}/items/{productId}", Name = "RemoveFromBasket")]
    [SwaggerOperation(
        Summary = "Sepetten ürün çıkar",
        Description = "Müşterinin sepetinden belirli bir ürünü çıkarır",
        OperationId = "RemoveFromBasket",
        Tags = new[] { "Basket" })]
    [SwaggerResponse(204, "Ürün sepetten çıkarıldı")]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(404, "Sepet veya ürün bulunamadı")]
    public async Task<IActionResult> RemoveFromBasket(
        string customerId,
        int productId,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new RemoveFromBasketCommand
            {
                CustomerId = customerId,
                ProductId = productId
            },
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Sepeti temizler
    /// </summary>
    [HttpDelete("{customerId}", Name = "ClearBasket")]
    [SwaggerOperation(
        Summary = "Sepeti temizle",
        Description = "Müşterinin tüm sepetini siler",
        OperationId = "ClearBasket",
        Tags = new[] { "Basket" })]
    [SwaggerResponse(204, "Sepet temizlendi")]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(404, "Sepet bulunamadı")]
    public async Task<IActionResult> ClearBasket(
        string customerId,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new ClearBasketCommand { CustomerId = customerId },
            cancellationToken);

        return NoContent();
    }
}