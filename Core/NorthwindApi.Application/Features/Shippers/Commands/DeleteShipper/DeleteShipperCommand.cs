using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Features.Shippers.Commands.DeleteShipper
{
    public class DeleteShipperCommand : IRequest<bool>
    {
        public int ShipperId { get; set; }
    }
}
