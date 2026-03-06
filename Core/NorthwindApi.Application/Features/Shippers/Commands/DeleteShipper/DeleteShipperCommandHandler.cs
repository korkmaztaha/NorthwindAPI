using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Shippers.Commands.DeleteShipper
{
    public class DeleteShipperCommandHandler : IRequestHandler<DeleteShipperCommand, bool>
    {
        private readonly IShipperService _shipperService;

        public DeleteShipperCommandHandler(IShipperService shipperService)
        {
            _shipperService = shipperService;
        }

        public async Task<bool> Handle(
            DeleteShipperCommand request,
            CancellationToken cancellationToken)
            => await _shipperService.DeleteAsync(request.ShipperId, cancellationToken);
    }
}
