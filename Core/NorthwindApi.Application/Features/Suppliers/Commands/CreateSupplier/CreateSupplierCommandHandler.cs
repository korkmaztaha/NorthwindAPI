using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Suppliers.Commands.CreateSupplier
{
    public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, CreateSupplierResponse>
    {
        private readonly ISupplierService _supplierService;

        public CreateSupplierCommandHandler(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        public async Task<CreateSupplierResponse> Handle(
            CreateSupplierCommand request,
            CancellationToken cancellationToken)
            => await _supplierService.CreateSupplierAsync(request, cancellationToken);
    }
}
