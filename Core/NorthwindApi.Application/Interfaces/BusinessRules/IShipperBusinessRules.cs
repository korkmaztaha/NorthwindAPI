using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.BusinessRules
{
    public interface IShipperBusinessRules
    {
        Task ShipperMustExistAsync(int shipperId, CancellationToken cancellationToken);
        Task ShipperCompanyNameMustBeUniqueAsync(string companyName, CancellationToken cancellationToken);
        Task ShipperHasNoOrdersAsync(int shipperId, CancellationToken cancellationToken);
    }
}
