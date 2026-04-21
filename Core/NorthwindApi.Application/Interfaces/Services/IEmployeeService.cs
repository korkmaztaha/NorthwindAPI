using NorthwindApi.Application.Features.Employees.Commands.CreateEmployee;
using NorthwindApi.Application.Features.Employees.Commands.DeleteEmployee;
using NorthwindApi.Application.Features.Employees.Commands.UpdateEmployee;
using NorthwindApi.Application.Features.Employees.Queries.GetEmployees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        Task<List<GetEmployeesResponse>> GetEmployeesAsync(GetEmployeesQuery request, CancellationToken cancellationToken);
        Task<CreateEmployeeResponse> CreateEmployeeAsync(CreateEmployeeCommand request, CancellationToken cancellationToken);
        Task<UpdateEmployeeResponse> UpdateEmployeeAsync(UpdateEmployeeCommand request, CancellationToken cancellationToken);
        Task DeleteEmployeeAsync(DeleteEmployeeCommand request, CancellationToken cancellationToken);
    }
}
