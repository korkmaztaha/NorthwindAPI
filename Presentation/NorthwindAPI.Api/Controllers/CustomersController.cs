using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Application.Features.Customers.Commands.CreateCustomer;
using NorthwindApi.Application.Features.Customers.Commands.DeleteCustomer;
using NorthwindApi.Application.Features.Customers.Commands.UpdateCustomer;
using NorthwindApi.Application.Features.Customers.Queries.GetCustomerOrderSummary;
using NorthwindApi.Application.Features.Customers.Queries.GetCustomers;
using NorthwindApi.Domain.Constants;

namespace NorthwindAPI.Api.Controllers;

[Authorize] 
[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }


    //[Authorize(Roles = Roles.Admin)]
    [HttpGet("GetCustomers", Name = "GetCustomers")]
    public async Task<IActionResult> GetCustomers(
    [FromQuery] GetCustomersQuery query,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }


    [HttpPost("CreateCustomer", Name = "CreateCustomer")]
     public async Task<IActionResult> CreateCustomer(
    [FromBody] CreateCustomerCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(
            nameof(GetCustomers),
            new { id = result.CustomerId },
            result);
    }

    [HttpPut("UpdateCustomer", Name = "UpdateCustomer")]
    public async Task<IActionResult> UpdateCustomer(
    [FromBody] UpdateCustomerCommand command,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    [HttpDelete("DeleteCustomer/{customerId}", Name = "DeleteCustomer")]
    public async Task<IActionResult> DeleteCustomer(
    string customerId,
    CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteCustomerCommand { CustomerId = customerId }, cancellationToken);
        return NoContent();
    }

    [HttpGet("GetCustomerOrderSummary", Name = "GetCustomerOrderSummary")]
    public async Task<IActionResult> GetCustomerOrderSummary(
    [FromQuery] GetCustomerOrderSummaryQuery query,
    CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}