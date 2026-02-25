using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandResponse
    {
        public string CustomerId { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
    }
}
