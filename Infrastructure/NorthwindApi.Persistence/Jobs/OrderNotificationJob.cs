using Microsoft.Extensions.Logging;
using NorthwindApi.Application.Interfaces.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Persistence.Jobs
{
    public class OrderNotificationJob : IOrderNotificationJob
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<OrderNotificationJob> _logger;

        public OrderNotificationJob(IEmailService emailService, ILogger<OrderNotificationJob> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task SendOrderConfirmationEmailAsync(
            int orderId,
            string customerEmail,
            string companyName)
        {
            try
            {
                _logger.LogInformation("Sipariş onay emaili gönderiliyor. OrderId: {OrderId}", orderId);

                await _emailService.SendOrderConfirmationAsync(orderId, customerEmail, companyName);

                _logger.LogInformation("Sipariş onay emaili gönderildi. OrderId: {OrderId}", orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sipariş onay emaili gönderilemedi. OrderId: {OrderId}", orderId);
                throw;
            }
        }
    }
}
