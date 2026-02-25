using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Customers.Queries.GetCustomerOrderSummary
{
    public class GetCustomerOrderSummaryResponse
    {
        public string CustomerId { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string? City { get; set; }
        public string? Country { get; set; }
        public int OrderCount { get; set; }          
        public decimal TotalSpent { get; set; }     
        public DateTime? LastOrderDate { get; set; } 
        public bool HasOrders { get; set; }
    }
}
