using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Orders.Commands.CreateOrder
{

    public class CreateOrderCommandResponse
    {
        public int OrderId { get; set; }
        public string? CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<CreateOrderItemResponse> Items { get; set; } = new();
    }

    public class CreateOrderItemResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public short Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public float Discount { get; set; }
        public decimal LineTotal { get; set; }
    }
}
