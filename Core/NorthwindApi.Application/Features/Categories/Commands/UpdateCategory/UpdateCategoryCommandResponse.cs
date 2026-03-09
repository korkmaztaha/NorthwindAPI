using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandResponse
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
    }
}
