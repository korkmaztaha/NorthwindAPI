using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NorthwindApi.Application.Events;
using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NorthwindApi.Persistence.Jobs
{
    public class OutboxProcessor
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly ILogger<OutboxProcessor> _logger;

        public OutboxProcessor(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            ILogger<OutboxProcessor> logger)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task ProcessAsync()
        {
            // İşlenmemiş mesajları çek (max 20 adet)
            var messages = await _unitOfWork.Repository<OutboxMessage>()
                .GetAll()
                .Where(m => m.ProcessedAt == null && m.RetryCount < 3)
                .OrderBy(m => m.CreatedAt)
                .Take(20)
                .ToListAsync();

            if (!messages.Any())
            {
                _logger.LogInformation("İşlenecek outbox mesajı yok.");
                return;
            }

            _logger.LogInformation("{Count} outbox mesajı işlenecek.", messages.Count);

            foreach (var message in messages)
            {
                try
                {
                    await ProcessMessageAsync(message);

                    message.ProcessedAt = DateTime.UtcNow;
                    message.Error = null;

                    _logger.LogInformation("Outbox mesajı işlendi. Id: {Id}, Type: {Type}",
                        message.Id, message.Type);
                }
                catch (Exception ex)
                {
                    message.RetryCount++;
                    message.Error = ex.Message;

                    _logger.LogError(ex, "Outbox mesajı işlenemedi. Id: {Id}, Type: {Type}, RetryCount: {RetryCount}",
                        message.Id, message.Type, message.RetryCount);
                }
                finally
                {
                    _unitOfWork.Repository<OutboxMessage>().Update(message);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken: default);
        }

        private async Task ProcessMessageAsync(OutboxMessage message)
        {
            switch (message.Type)
            {
                case nameof(OrderCreatedEvent):
                    var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message.Payload)
                        ?? throw new InvalidOperationException("OrderCreatedEvent deserialize edilemedi.");

                    await _emailService.SendOrderConfirmationAsync(
                        orderEvent.OrderId,
                        orderEvent.CustomerEmail,
                        orderEvent.CompanyName);
                    break;

                default:
                    _logger.LogWarning("Bilinmeyen outbox mesaj tipi: {Type}", message.Type);
                    break;
            }
        }
    }

}