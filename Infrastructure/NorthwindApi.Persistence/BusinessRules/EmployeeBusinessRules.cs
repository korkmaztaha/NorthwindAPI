using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Interfaces.BusinessRules;
using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Persistence.BusinessRules
{
    public class EmployeeBusinessRules : IEmployeeBusinessRules
    {
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeBusinessRules(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task EmployeeMustExistAsync(int employeeId, CancellationToken cancellationToken = default)
        {
            var exists = await _unitOfWork.Repository<Employee>()
                .GetAll()
                .AnyAsync(e => e.EmployeeId == employeeId, cancellationToken);

            if (!exists)
                throw new KeyNotFoundException($"Employee with ID {employeeId} not found.");
        }

        public async Task ReportsToEmployeeMustExistAsync(int? reportsTo, CancellationToken cancellationToken = default)
        {
            if (!reportsTo.HasValue) return;

            var exists = await _unitOfWork.Repository<Employee>()
                .GetAll()
                .AnyAsync(e => e.EmployeeId == reportsTo.Value, cancellationToken);

            if (!exists)
                throw new KeyNotFoundException($"Manager with ID {reportsTo} not found.");
        }

        public async Task EmployeeHasNoOrdersAsync(int employeeId, CancellationToken cancellationToken = default)
        {
            var hasOrders = await _unitOfWork.Repository<Orders>()
                .GetAll()
                .AnyAsync(o => o.EmployeeId == employeeId, cancellationToken);

            if (hasOrders)
                throw new InvalidOperationException($"Employee with ID {employeeId} has orders and cannot be deleted.");
        }
    }
}