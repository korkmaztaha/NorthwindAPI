using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Customers.Commands.CreateCustomer
{
    public class CreateCustomerCommandResponse
    {
        public string CustomerId { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
