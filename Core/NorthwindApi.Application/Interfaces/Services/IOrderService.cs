using NorthwindApi.Application.Features.Orders.Queries.GetOrderDetail;
using NorthwindApi.Application.Features.Orders.Queries.GetOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.Services
{

    public interface IOrderService
    {
        Task<List<GetOrdersQueryResponse>> GetAllAsync(GetOrdersQuery request, CancellationToken cancellationToken);
        Task<GetOrderDetailResponse> GetDetailAsync(int orderId, CancellationToken cancellationToken);
    }
}
