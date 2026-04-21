using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.BusinessRules
{
    public interface IEmployeeBusinessRules
    {
        Task EmployeeMustExistAsync(int employeeId, CancellationToken cancellationToken = default);
        Task ReportsToEmployeeMustExistAsync(int? reportsTo, CancellationToken cancellationToken = default);
        Task EmployeeHasNoOrdersAsync(int employeeId, CancellationToken cancellationToken = default);
    }
}
