using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Products.Queries.GetProducts
{
    public class GetProductsQueryResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? CategoryName { get; set; }
        public string? SupplierName { get; set; }
        public string? QuantityPerUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public short? UnitsOnOrder { get; set; }
        public short? ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
    }
}
