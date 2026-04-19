namespace NorthwindApi.Application.Features.Suppliers.Commands.CreateSupplier
{
    public class CreateSupplierResponse
    {
        public int SupplierId { get; set; }
        public string CompanyName { get; set; } = null!;
    }
}