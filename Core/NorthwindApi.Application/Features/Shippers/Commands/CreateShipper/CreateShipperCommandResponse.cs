using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Shippers.Commands.CreateShipper
{
    public class CreateShipperCommandResponse
    {
        public int ShipperId { get; set; }
        public string CompanyName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
