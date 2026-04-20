using MediatR;
using NorthwindApi.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Suppliers.Commands.DeleteSupplier
{

    public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand>
    {
        private readonly ISupplierService _supplierService;

        public DeleteSupplierCommandHandler(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        public async Task Handle(
            DeleteSupplierCommand request,
            CancellationToken cancellationToken)
            => await _supplierService.DeleteSupplierAsync(request, cancellationToken);
    }
}
