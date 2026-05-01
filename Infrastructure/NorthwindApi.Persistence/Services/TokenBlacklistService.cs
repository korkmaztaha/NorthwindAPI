using NorthwindApi.Application.Interfaces.Infrastructure;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Persistence.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly IConnectionMultiplexer _redis;
        private const string Prefix = "blacklist:";

        public TokenBlacklistService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task BlacklistTokenAsync(string jti, DateTime expiry, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            var ttl = expiry - DateTime.UtcNow;

            if (ttl > TimeSpan.Zero)
                await db.StringSetAsync($"{Prefix}{jti}", "blacklisted", ttl);
        }

        public async Task<bool> IsTokenBlacklistedAsync(string jti, CancellationToken cancellationToken = default)
        {
            var db = _redis.GetDatabase();
            return await db.KeyExistsAsync($"{Prefix}{jti}");
        }
    }
}
