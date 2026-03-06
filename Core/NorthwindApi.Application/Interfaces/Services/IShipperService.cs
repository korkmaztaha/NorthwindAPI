using NorthwindApi.Application.Features.Shippers.Commands.CreateShipper;
using NorthwindApi.Application.Features.Shippers.Commands.UpdateShipper;
using NorthwindApi.Application.Features.Shippers.Queries.GetShippers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.Services
{

    public interface IShipperService
    {
        Task<List<GetShippersQueryResponse>> GetAllAsync(GetShippersQuery request, CancellationToken cancellationToken);
        Task<CreateShipperCommandResponse> CreateAsync(CreateShipperCommand request, CancellationToken cancellationToken);
        Task<UpdateShipperCommandResponse> UpdateAsync(UpdateShipperCommand request, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int shipperId, CancellationToken cancellationToken);
    }
}
