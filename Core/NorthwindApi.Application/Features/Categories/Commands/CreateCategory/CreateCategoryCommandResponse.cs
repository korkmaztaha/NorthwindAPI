using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommandResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public List<CreateCategoryProductResponse> Products { get; set; } = new();
    }

    public class CreateCategoryProductResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal? UnitPrice { get; set; }
    }
}
