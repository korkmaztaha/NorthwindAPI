using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Orders.Commands.CreateOrder
{

    public class CreateOrderCommand : IRequest<CreateOrderCommandResponse>
    {
        public string CustomerId { get; set; } = null!;
        public int? EmployeeId { get; set; }
        public int? ShipVia { get; set; }
        public DateTime? RequiredDate { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipRegion { get; set; }
        public string? ShipPostalCode { get; set; }
        public string? ShipCountry { get; set; }
        public decimal? Freight { get; set; }
        public List<CreateOrderItemCommand> Items { get; set; } = new();
    }

    public class CreateOrderItemCommand
    {
        public int ProductId { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }
    }
}
