using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Domain.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Type { get; set; } = null!;
        public string Payload { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAt { get; set; }
        public string? Error { get; set; }
        public int RetryCount { get; set; } = 0;
    }
}
