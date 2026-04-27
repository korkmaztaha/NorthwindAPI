namespace NorthwindApi.Application.Features.Reports.GetSalesByEmployee
{
    public class GetSalesByEmployeeResponse
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Title { get; set; }
        public string? ReportsToName { get; set; }
        public int TotalOrders { get; set; }
        public int TotalItemsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public string? TopCategory { get; set; }
        public string? TopCustomer { get; set; }
    }
}