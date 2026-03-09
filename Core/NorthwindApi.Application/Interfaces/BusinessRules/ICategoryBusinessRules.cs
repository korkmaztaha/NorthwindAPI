using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.BusinessRules
{
    public interface ICategoryBusinessRules
    {
        Task CategoryMustExistAsync(int categoryId, CancellationToken cancellationToken);
        Task CategoryNameMustBeUniqueAsync(string categoryName, CancellationToken cancellationToken);
        Task CategoryHasNoProductsAsync(int categoryId, CancellationToken cancellationToken);
    }

}
