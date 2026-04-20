using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.BusinessRules
{
    public interface ISupplierBusinessRules
    {
        Task SupplierMustExistAsync(int supplierId, CancellationToken cancellationToken = default);
        Task SupplierCompanyNameMustBeUniqueAsync(string companyName, CancellationToken cancellationToken = default);
        Task SupplierHasNoProductsAsync(int supplierId, CancellationToken cancellationToken = default);
    }
}
