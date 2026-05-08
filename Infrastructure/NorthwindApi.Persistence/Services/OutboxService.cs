using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NorthwindApi.Persistence.Services
{
    public class OutboxService : IOutboxService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OutboxService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddMessageAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            var outboxMessage = new OutboxMessage
            {
                Type = typeof(T).Name,
                Payload = JsonSerializer.Serialize(message),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<OutboxMessage>()
                .AddAsync(outboxMessage, cancellationToken);
        }
    }
}
