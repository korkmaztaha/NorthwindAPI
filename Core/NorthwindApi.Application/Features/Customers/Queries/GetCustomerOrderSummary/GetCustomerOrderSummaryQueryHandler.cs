
using MediatR;
using NorthwindApi.Application.Interfaces.Services;

namespace NorthwindApi.Application.Features.Customers.Queries.GetCustomerOrderSummary;

public class GetCustomerOrderSummaryQueryHandler
    : IRequestHandler<GetCustomerOrderSummaryQuery, List<GetCustomerOrderSummaryResponse>>
{
    private readonly ICustomerService _customerService;

    public GetCustomerOrderSummaryQueryHandler(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    public async Task<List<GetCustomerOrderSummaryResponse>> Handle(
        GetCustomerOrderSummaryQuery request,
        CancellationToken cancellationToken)
        => await _customerService.GetOrderSummaryAsync(request, cancellationToken);
}