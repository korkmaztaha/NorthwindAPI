using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Shippers.Commands.UpdateShipper
{
    public class UpdateShipperCommandHandler : IRequestHandler<UpdateShipperCommand, UpdateShipperCommandResponse>
    {
        private readonly IShipperService _shipperService;

        public UpdateShipperCommandHandler(IShipperService shipperService)
        {
            _shipperService = shipperService;
        }

        public async Task<UpdateShipperCommandResponse> Handle(
            UpdateShipperCommand request,
            CancellationToken cancellationToken)
            => await _shipperService.UpdateAsync(request, cancellationToken);
    }
}
