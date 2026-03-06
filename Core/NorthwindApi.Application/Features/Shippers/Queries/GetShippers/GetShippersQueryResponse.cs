using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Shippers.Queries.GetShippers
{
    public class GetShippersQueryResponse
    {
        public int ShipperId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? Phone { get; set; }
        public int TotalOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int PendingOrders { get; set; }
        public int DelayedOrders { get; set; }
        public decimal TotalFreight { get; set; }
        public decimal AverageFreight { get; set; }
    }
}
