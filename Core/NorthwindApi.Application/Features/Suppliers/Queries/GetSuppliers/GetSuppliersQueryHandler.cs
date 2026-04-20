using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Suppliers.Queries.GetSuppliers
{

    public class GetSuppliersQueryHandler : IRequestHandler<GetSuppliersQuery, List<GetSuppliersResponse>>
    {
        private readonly ISupplierService _supplierService;

        public GetSuppliersQueryHandler(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        public async Task<List<GetSuppliersResponse>> Handle(
            GetSuppliersQuery request,
            CancellationToken cancellationToken)
            => await _supplierService.GetSuppliersAsync(request, cancellationToken);
    }
}
