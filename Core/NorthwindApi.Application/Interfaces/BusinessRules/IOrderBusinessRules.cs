using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.BusinessRules
{
    public interface IOrderBusinessRules
    {
        Task OrderMustExistAsync(int orderId, CancellationToken cancellationToken);
    }
}
