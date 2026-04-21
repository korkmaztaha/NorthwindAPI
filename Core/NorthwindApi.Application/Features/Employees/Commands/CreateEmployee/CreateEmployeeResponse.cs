namespace NorthwindApi.Application.Features.Employees.Commands.CreateEmployee
{
    public class CreateEmployeeResponse
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = null!;
    }
}