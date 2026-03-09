using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetSalesReport.GetSalesByPeriod
{
    public class GetSalesByPeriodResponse
    {
        public int Year { get; set; }
        public int? Month { get; set; }
        public string? MonthName { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSales { get; set; }
        public decimal TotalFreight { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalItemsSold { get; set; }
    }
}
