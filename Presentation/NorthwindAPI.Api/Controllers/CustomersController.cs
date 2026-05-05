using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NorthwindApi.Application.Features.Customers.Commands.CreateCustomer;
using NorthwindApi.Application.Features.Customers.Commands.DeleteCustomer;
using NorthwindApi.Application.Features.Customers.Commands.UpdateCustomer;
using NorthwindApi.Application.Features.Customers.Queries.GetCustomers;
using Swashbuckle.AspNetCore.Annotations;

/// <summary>
/// Müşteri yönetimi işlemleri
/// </summary>
[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[EnableRateLimiting("GeneralPolicy")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Müşteri listesini getirir
    /// </summary>
    /// <remarks>
    /// Filtreleme ve sayfalama destekler.
    /// </remarks>
    [HttpGet("GetCustomers", Name = "GetCustomers")]
    [SwaggerOperation(
        Summary = "Müşteri listesi",
        Description = "Filtreleme ve sayfalama ile müşteri listesi getirir",
        OperationId = "GetCustomers",
        Tags = new[] { "Customers" })]
    [SwaggerResponse(200, "Başarılı", typeof(List<GetCustomersQueryResponse>))]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> GetCustomers(
        [FromQuery] GetCustomersQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Yeni müşteri oluşturur
    /// </summary>
    [HttpPost("CreateCustomer", Name = "CreateCustomer")]
    [SwaggerOperation(
        Summary = "Müşteri oluştur",
        Description = "Yeni bir müşteri kaydı oluşturur",
        OperationId = "CreateCustomer",
        Tags = new[] { "Customers" })]
    [SwaggerResponse(201, "Müşteri oluşturuldu", typeof(CreateCustomerCommandResponse))]
    [SwaggerResponse(400, "Geçersiz istek veya validation hatası")]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> CreateCustomer(
        [FromBody] CreateCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetCustomers), new { id = result.CustomerId }, result);
    }

    /// <summary>
    /// Müşteri günceller
    /// </summary>
    [HttpPut("UpdateCustomer", Name = "UpdateCustomer")]
    [SwaggerOperation(
        Summary = "Müşteri güncelle",
        Description = "Mevcut müşteri bilgilerini günceller",
        OperationId = "UpdateCustomer",
        Tags = new[] { "Customers" })]
    [SwaggerResponse(200, "Müşteri güncellendi", typeof(UpdateCustomerCommandResponse))]
    [SwaggerResponse(400, "Geçersiz istek veya validation hatası")]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(404, "Müşteri bulunamadı")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> UpdateCustomer(
        [FromBody] UpdateCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Müşteri siler
    /// </summary>
    [HttpDelete("DeleteCustomer/{customerId}", Name = "DeleteCustomer")]
    [SwaggerOperation(
        Summary = "Müşteri sil",
        Description = "Müşteriyi kalıcı olarak siler",
        OperationId = "DeleteCustomer",
        Tags = new[] { "Customers" })]
    [SwaggerResponse(204, "Müşteri silindi")]
    [SwaggerResponse(401, "Yetkisiz erişim")]
    [SwaggerResponse(404, "Müşteri bulunamadı")]
    [SwaggerResponse(429, "Çok fazla istek")]
    public async Task<IActionResult> DeleteCustomer(
        string customerId,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCustomerCommand { CustomerId = customerId }, cancellationToken);
        return NoContent();
    }
}