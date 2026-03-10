using NorthwindApi.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetEmployeePerformance
{
    public class GetEmployeePerformanceResponse
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Title { get; set; }
        public string? ReportsToName { get; set; }      // Kime bağlı

        // Sipariş bilgileri
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalItemsSold { get; set; }

        // En çok sattığı kategori
        public string? TopCategory { get; set; }

        // En çok sattığı müşteri
        public string? TopCustomer { get; set; }

        // Trend
        public List<EmployeeMonthlyTrend> MonthlyTrends { get; set; } = new();
    }

}
