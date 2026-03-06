using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Shippers.Commands.CreateShipper
{
    public class CreateShipperCommandHandler : IRequestHandler<CreateShipperCommand, CreateShipperCommandResponse>
    {
        private readonly IShipperService _shipperService;

        public CreateShipperCommandHandler(IShipperService shipperService)
        {
            _shipperService = shipperService;
        }

        public async Task<CreateShipperCommandResponse> Handle(
            CreateShipperCommand request,
            CancellationToken cancellationToken)
            => await _shipperService.CreateAsync(request, cancellationToken);
    }
}
