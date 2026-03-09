using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Categories.Queries.GetCategoryDetail
{
    public class GetCategoryDetailQuery : IRequest<GetCategoryDetailQueryResponse>
    {
        public int CategoryId { get; set; }
    }
}
