using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.Infrastructure
{
    public interface ITokenBlacklistService
    {
        Task BlacklistTokenAsync(string jti, DateTime expiry, CancellationToken cancellationToken = default);
        Task<bool> IsTokenBlacklistedAsync(string jti, CancellationToken cancellationToken = default);
    }
}
