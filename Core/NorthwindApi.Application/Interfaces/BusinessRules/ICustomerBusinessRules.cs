using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.BusinessRules
{
    public interface ICustomerBusinessRules
    {
        Task CustomerMustExistAsync(string customerId, CancellationToken cancellationToken);
        Task CustomerIdMustBeUniqueAsync(string customerId, CancellationToken cancellationToken);
    }
}
