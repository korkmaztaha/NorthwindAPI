using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Shippers.Commands.CreateShipper
{
    public class CreateShipperCommand : IRequest<CreateShipperCommandResponse>
    {
        public string CompanyName { get; set; } = null!;
        public string? Phone { get; set; }
    }
}
