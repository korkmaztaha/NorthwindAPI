using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Events
{
    public class OrderCreatedEvent
    {
        public int OrderId { get; set; }
        public string CustomerId { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
