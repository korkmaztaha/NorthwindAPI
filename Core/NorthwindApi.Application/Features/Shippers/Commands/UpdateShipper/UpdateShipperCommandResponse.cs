using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Shippers.Commands.UpdateShipper
{
    public class UpdateShipperCommandResponse
    {
        public int ShipperId { get; set; }
        public string CompanyName { get; set; } = null!;
        public DateTime UpdatedAt { get; set; }
    }
}
