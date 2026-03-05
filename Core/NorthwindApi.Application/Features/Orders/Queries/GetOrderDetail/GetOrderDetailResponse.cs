using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Orders.Queries.GetOrderDetail
{

    public class GetOrderDetailResponse
    {
        public int OrderId { get; set; }
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? EmployeeFullName { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public string? ShipperName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipCountry { get; set; }
        public decimal? Freight { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderDetailItemResponse> Items { get; set; } = new();
    }

    public class OrderDetailItemResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? CategoryName { get; set; }
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
        public decimal LineTotal { get; set; } 
    }
}
