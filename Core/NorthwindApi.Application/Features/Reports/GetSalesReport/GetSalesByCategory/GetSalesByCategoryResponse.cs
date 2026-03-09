using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByCategory
{
    public class GetSalesByCategoryResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public int TotalOrders { get; set; }
        public int TotalProductsSold { get; set; }
        public decimal TotalSales { get; set; }
        public decimal AverageOrderValue { get; set; }
        public string TopSellingProduct { get; set; } = null!;
    }
}
