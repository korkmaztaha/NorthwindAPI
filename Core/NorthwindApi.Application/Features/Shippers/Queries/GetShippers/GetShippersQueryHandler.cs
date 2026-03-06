using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Shippers.Queries.GetShippers
{
    public class GetShippersQueryHandler : IRequestHandler<GetShippersQuery, List<GetShippersQueryResponse>>
    {
        private readonly IShipperService _shipperService;

        public GetShippersQueryHandler(IShipperService shipperService)
        {
            _shipperService = shipperService;
        }

        public async Task<List<GetShippersQueryResponse>> Handle(
            GetShippersQuery request,
            CancellationToken cancellationToken)
            => await _shipperService.GetAllAsync(request, cancellationToken);
    }
}
