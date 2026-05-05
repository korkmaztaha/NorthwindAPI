using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.Infrastructure
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(int orderId, string customerEmail, string companyName, CancellationToken cancellationToken = default);
    }
}
