using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Features.Employees.Commands.CreateEmployee;
using NorthwindApi.Application.Features.Employees.Commands.DeleteEmployee;
using NorthwindApi.Application.Features.Employees.Commands.UpdateEmployee;
using NorthwindApi.Application.Features.Employees.Queries.GetEmployees;
using NorthwindApi.Application.Interfaces.BusinessRules;
using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Application.Interfaces.Services;
using NorthwindApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Persistence.Services.EntityServices
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeBusinessRules _businessRules;

        public EmployeeService(IUnitOfWork unitOfWork, IEmployeeBusinessRules businessRules)
        {
            _unitOfWork = unitOfWork;
            _businessRules = businessRules;
        }

        public async Task<List<GetEmployeesResponse>> GetEmployeesAsync(
            GetEmployeesQuery request,
            CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Employees>().GetAll();

            if (!string.IsNullOrEmpty(request.FirstName))
                query = query.Where(e => e.FirstName.Contains(request.FirstName));

            if (!string.IsNullOrEmpty(request.LastName))
                query = query.Where(e => e.LastName.Contains(request.LastName));

            if (!string.IsNullOrEmpty(request.Title))
                query = query.Where(e => e.Title!.Contains(request.Title));

            if (!string.IsNullOrEmpty(request.Country))
                query = query.Where(e => e.Country == request.Country);

            return await query
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new GetEmployeesResponse
                {
                    EmployeeId = e.EmployeeId,
                    FullName = e.FirstName + " " + e.LastName,
                    Title = e.Title,
                    TitleOfCourtesy = e.TitleOfCourtesy,
                    BirthDate = e.BirthDate,
                    HireDate = e.HireDate,
                    Address = e.Address,
                    City = e.City,
                    Country = e.Country,
                    HomePhone = e.HomePhone,
                    ReportsToName = e.ReportsToNavigation != null
                        ? e.ReportsToNavigation.FirstName + " " + e.ReportsToNavigation.LastName
                        : null,
                    TotalOrders = e.Orders.Count()
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<CreateEmployeeResponse> CreateEmployeeAsync(
            CreateEmployeeCommand request,
            CancellationToken cancellationToken)
        {
            await _businessRules.ReportsToEmployeeMustExistAsync(request.ReportsTo, cancellationToken);

            var employee = new Employees
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Title = request.Title,
                TitleOfCourtesy = request.TitleOfCourtesy,
                BirthDate = request.BirthDate,
                HireDate = request.HireDate,
                Address = request.Address,
                City = request.City,
                Region = request.Region,
                PostalCode = request.PostalCode,
                Country = request.Country,
                HomePhone = request.HomePhone,
                Extension = request.Extension,
                Notes = request.Notes,
                ReportsTo = request.ReportsTo,
                PhotoPath = request.PhotoPath
            };

            await _unitOfWork.Repository<Employees>().AddAsync(employee, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateEmployeeResponse
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FirstName + " " + employee.LastName
            };
        }

        public async Task<UpdateEmployeeResponse> UpdateEmployeeAsync(
            UpdateEmployeeCommand request,
            CancellationToken cancellationToken)
        {
            await _businessRules.EmployeeMustExistAsync(request.EmployeeId, cancellationToken);
            await _businessRules.ReportsToEmployeeMustExistAsync(request.ReportsTo, cancellationToken);

            var employee = await _unitOfWork.Repository<Employees>()
                .GetAll()
                .FirstAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Title = request.Title;
            employee.TitleOfCourtesy = request.TitleOfCourtesy;
            employee.BirthDate = request.BirthDate;
            employee.HireDate = request.HireDate;
            employee.Address = request.Address;
            employee.City = request.City;
            employee.Region = request.Region;
            employee.PostalCode = request.PostalCode;
            employee.Country = request.Country;
            employee.HomePhone = request.HomePhone;
            employee.Extension = request.Extension;
            employee.Notes = request.Notes;
            employee.ReportsTo = request.ReportsTo;
            employee.PhotoPath = request.PhotoPath;

            _unitOfWork.Repository<Employees>().Update(employee);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateEmployeeResponse
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FirstName + " " + employee.LastName
            };
        }

        public async Task DeleteEmployeeAsync(
            DeleteEmployeeCommand request,
            CancellationToken cancellationToken)
        {
            await _businessRules.EmployeeMustExistAsync(request.EmployeeId, cancellationToken);
            await _businessRules.EmployeeHasNoOrdersAsync(request.EmployeeId, cancellationToken);

            var employee = await _unitOfWork.Repository<Employees>()
                .GetAll()
                .FirstAsync(e => e.EmployeeId == request.EmployeeId, cancellationToken);

            _unitOfWork.Repository<Employees>().Delete(employee);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}