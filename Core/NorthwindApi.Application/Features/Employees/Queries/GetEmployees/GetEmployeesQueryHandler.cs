using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Employees.Queries.GetEmployees
{
    public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, List<GetEmployeesResponse>>
    {
        private readonly IEmployeeService _employeeService;

        public GetEmployeesQueryHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<List<GetEmployeesResponse>> Handle(
            GetEmployeesQuery request,
            CancellationToken cancellationToken)
            => await _employeeService.GetEmployeesAsync(request, cancellationToken);
    }
}
