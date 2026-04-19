namespace NorthwindApi.Application.Features.Suppliers.Commands.UpdateSupplier
{

    public class UpdateSupplierResponse
    {
        public int SupplierId { get; set; }
        public string CompanyName { get; set; } = null!;
    }
}