using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Features.Shippers.Commands.CreateShipper;
using NorthwindApi.Application.Features.Shippers.Commands.UpdateShipper;
using NorthwindApi.Application.Features.Shippers.Queries.GetShippers;
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

    public class ShipperService : IShipperService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShipperBusinessRules _businessRules;

        public ShipperService(IUnitOfWork unitOfWork, IShipperBusinessRules businessRules)
        {
            _unitOfWork = unitOfWork;
            _businessRules = businessRules;
        }

        public async Task<List<GetShippersQueryResponse>> GetAllAsync(
            GetShippersQuery request,
            CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Repository<Shippers>().GetAll();

            if (!string.IsNullOrEmpty(request.CompanyName))
                query = query.Where(x => x.CompanyName.Contains(request.CompanyName));

            return await query
                .OrderBy(x => x.CompanyName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new GetShippersQueryResponse
                {
                    ShipperId = x.ShipperId,
                    CompanyName = x.CompanyName,
                    Phone = x.Phone,
                    TotalOrders = x.Orders.Count,
                    DeliveredOrders = x.Orders.Count(o => o.ShippedDate != null),
                    PendingOrders = x.Orders.Count(o => o.ShippedDate == null),
                    DelayedOrders = x.Orders.Count(o =>
                        o.ShippedDate == null &&
                        o.RequiredDate < DateTime.UtcNow),
                    TotalFreight = x.Orders.Sum(o => o.Freight ?? 0),
                    AverageFreight = x.Orders.Any()
                        ? x.Orders.Average(o => (decimal?)o.Freight) ?? 0
                        : 0
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<CreateShipperCommandResponse> CreateAsync(
            CreateShipperCommand request,
            CancellationToken cancellationToken)
        {
            await _businessRules.ShipperCompanyNameMustBeUniqueAsync(request.CompanyName, cancellationToken);

            var shipper = new Shippers
            {
                CompanyName = request.CompanyName,
                Phone = request.Phone
            };

            await _unitOfWork.Repository<Shippers>().AddAsync(shipper, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateShipperCommandResponse
            {
                ShipperId = shipper.ShipperId,
                CompanyName = shipper.CompanyName,
                CreatedAt = DateTime.UtcNow
            };
        }

        public async Task<UpdateShipperCommandResponse> UpdateAsync(
            UpdateShipperCommand request,
            CancellationToken cancellationToken)
        {
            await _businessRules.ShipperMustExistAsync(request.ShipperId, cancellationToken);

            var shipper = await _unitOfWork.Repository<Shippers>()
                .GetAll()
                .FirstOrDefaultAsync(x => x.ShipperId == request.ShipperId, cancellationToken);

            shipper!.CompanyName = request.CompanyName;
            shipper.Phone = request.Phone;

            _unitOfWork.Repository<Shippers>().Update(shipper);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateShipperCommandResponse
            {
                ShipperId = shipper.ShipperId,
                CompanyName = shipper.CompanyName,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<bool> DeleteAsync(int shipperId, CancellationToken cancellationToken)
        {
            await _businessRules.ShipperMustExistAsync(shipperId, cancellationToken);
            await _businessRules.ShipperHasNoOrdersAsync(shipperId, cancellationToken);

            var shipper = await _unitOfWork.Repository<Shippers>()
                .GetAll()
                .FirstOrDefaultAsync(x => x.ShipperId == shipperId, cancellationToken);

            _unitOfWork.Repository<Shippers>().Delete(shipper!);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
