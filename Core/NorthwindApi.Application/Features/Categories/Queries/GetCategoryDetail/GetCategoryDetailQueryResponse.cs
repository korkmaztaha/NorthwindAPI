using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Categories.Queries.GetCategoryDetail
{

    public class GetCategoryDetailQueryResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public int TotalProducts { get; set; }
        public List<CategoryProductResponse> Products { get; set; } = new();
    }

    public class CategoryProductResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal? UnitPrice { get; set; }
        public short? UnitsInStock { get; set; }
        public bool Discontinued { get; set; }
    }
}
