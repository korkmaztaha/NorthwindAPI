using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Interfaces.Infrastructure
{
    public interface IOutboxService
    {
        Task AddMessageAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
    }
}
