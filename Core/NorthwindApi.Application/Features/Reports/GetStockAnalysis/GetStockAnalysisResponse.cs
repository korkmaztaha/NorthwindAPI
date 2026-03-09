using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Reports.GetStockAnalysis
{
    public class GetStockAnalysisResponse
    {
        public List<StockAnalysisItemResponse> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public string AnalysisType { get; set; } = null!;
    }

    public class StockAnalysisItemResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? CategoryName { get; set; }
        public string? SupplierName { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public decimal? UnitPrice { get; set; }
        public bool Discontinued { get; set; }

        // Kritik stok için
        public short? StockDeficit { get; set; }        // ReorderLevel - UnitsInStock

        // Fazla stok için
        public DateTime? LastSaleDate { get; set; }
        public int? DaysSinceLastSale { get; set; }

        // Stok devir hızı için
        public int? TotalSoldLast12Months { get; set; }
        public decimal? TurnoverRate { get; set; }       // TotalSold / UnitsInStock

        // Discontinued için
        public int? TotalOrders { get; set; }
        public decimal? TotalRevenue { get; set; }
    }
}
