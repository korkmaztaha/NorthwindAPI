using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetTopSellingProducts
{
    public class GetTopSellingProductsResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }

      
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }

       
        public int? PreviousPeriodQuantity { get; set; }
        public decimal? PreviousPeriodRevenue { get; set; }
        public decimal? QuantityChangePercent { get; set; }    
        public decimal? RevenueChangePercent { get; set; }
        public string? TrendDirection { get; set; }   // Up, Down, Stable, New
    }
}
