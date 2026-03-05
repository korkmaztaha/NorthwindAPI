using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Orders.Queries.GetOrders
{
    public class GetOrdersQueryResponse
    {
        public int OrderId { get; set; }

        // Müşteri bilgisi
        public string? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerCity { get; set; }

        // Çalışan bilgisi
        public int? EmployeeId { get; set; }
        public string? EmployeeFullName { get; set; }
        public string? EmployeeTitle { get; set; }

        // Kargo bilgisi
        public string? ShipperName { get; set; }
        public string? ShipName { get; set; }
        public string? ShipCity { get; set; }
        public string? ShipCountry { get; set; }

        // Tarih bilgisi
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }

        public decimal? Freight { get; set; }

        // Hesaplanan alanlar
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
    }
}
