using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Suppliers.Commands.UpdateSupplier
{
    public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, UpdateSupplierResponse>
    {
        private readonly ISupplierService _supplierService;

        public UpdateSupplierCommandHandler(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        public async Task<UpdateSupplierResponse> Handle(
            UpdateSupplierCommand request,
            CancellationToken cancellationToken)
            => await _supplierService.UpdateSupplierAsync(request, cancellationToken);
    }
}
