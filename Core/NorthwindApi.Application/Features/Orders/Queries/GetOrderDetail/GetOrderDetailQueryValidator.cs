using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Orders.Queries.GetOrderDetail
{

    public class GetOrderDetailQueryValidator : AbstractValidator<GetOrderDetailQuery>
    {
        public GetOrderDetailQueryValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("Order ID must be greater than 0.");
        }
    }
}