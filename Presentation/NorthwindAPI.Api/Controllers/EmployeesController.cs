using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NorthwindApi.Application.Features.Employees.Commands.CreateEmployee;
using NorthwindApi.Application.Features.Employees.Commands.DeleteEmployee;
using NorthwindApi.Application.Features.Employees.Commands.UpdateEmployee;
using NorthwindApi.Application.Features.Employees.Queries.GetEmployees;

namespace NorthwindAPI.Api.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetEmployees", Name = "GetEmployees")]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] GetEmployeesQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpPost("CreateEmployee", Name = "CreateEmployee")]
        public async Task<IActionResult> CreateEmployee(
            [FromBody] CreateEmployeeCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetEmployees), new { id = result.EmployeeId }, result);
        }

        [HttpPut("UpdateEmployee", Name = "UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee(
            [FromBody] UpdateEmployeeCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("DeleteEmployee/{employeeId}", Name = "DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployee(
            int employeeId,
            CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteEmployeeCommand { EmployeeId = employeeId }, cancellationToken);
            return NoContent();
        }
    }
}