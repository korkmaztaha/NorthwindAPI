namespace NorthwindApi.Application.Features.Employees.Queries.GetEmployees
{
    public class GetEmployeesResponse
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Title { get; set; }
        public string? TitleOfCourtesy { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? HomePhone { get; set; }
        public string? ReportsToName { get; set; }
        public int TotalOrders { get; set; }
    }
}