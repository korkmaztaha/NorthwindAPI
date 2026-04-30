using NorthwindApi.Application.Features.Orders.Commands.CreateOrder;
using NorthwindApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.BusinessRules
{
    public interface IProductBusinessRules
    {
        Task ProductsMustExistAsync(List<int> productIds, CancellationToken cancellationToken);
        Task<List<Products>> GetAndValidateProductsAsync(List<CreateOrderItemCommand> items, CancellationToken cancellationToken);
        Task StockMustBeSufficientAsync(List<CreateOrderItemCommand> items, CancellationToken cancellationToken);
        Task ProductNameMustBeUniqueAsync(string productName, CancellationToken cancellationToken);
        Task ProductNameMustBeUniqueForUpdateAsync(int productId,string productName,CancellationToken cancellationToken);
    }
}
