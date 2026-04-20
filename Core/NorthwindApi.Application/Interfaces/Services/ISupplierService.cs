using NorthwindApi.Application.Features.Suppliers.Commands.CreateSupplier;
using NorthwindApi.Application.Features.Suppliers.Commands.DeleteSupplier;
using NorthwindApi.Application.Features.Suppliers.Commands.UpdateSupplier;
using NorthwindApi.Application.Features.Suppliers.Queries.GetSuppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.Services
{
    public interface ISupplierService
    {
        Task<List<GetSuppliersResponse>> GetSuppliersAsync(GetSuppliersQuery request, CancellationToken cancellationToken);
        Task<CreateSupplierResponse> CreateSupplierAsync(CreateSupplierCommand request, CancellationToken cancellationToken);
        Task<UpdateSupplierResponse> UpdateSupplierAsync(UpdateSupplierCommand request, CancellationToken cancellationToken);
        Task DeleteSupplierAsync(DeleteSupplierCommand request, CancellationToken cancellationToken);
    }
}
