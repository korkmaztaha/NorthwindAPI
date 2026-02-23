using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Application.Features.Customers.Queries.GetCustomers;

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

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var result = await _mediator.Send(new GetCustomersQuery());

        return Ok(result);
    }
}