using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
    }
}
