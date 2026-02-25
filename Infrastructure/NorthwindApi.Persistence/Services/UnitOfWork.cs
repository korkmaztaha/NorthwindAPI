using NorthwindApi.Application.Interfaces.Infrastructure;
using NorthwindApi.Application.Interfaces.Repositories;
using NorthwindApi.Persistence.Contexts;
using NorthwindApi.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Persistence.Services
{

    public class UnitOfWork : IUnitOfWork
    {
        private readonly NorthwindDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(NorthwindDbContext context) => _context = context;

        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (!_repositories.ContainsKey(typeof(T)))
                _repositories[typeof(T)] = new GenericRepository<T>(_context);

            return (IGenericRepository<T>)_repositories[typeof(T)];
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
            => await _context.SaveChangesAsync(cancellationToken);

        public void Dispose() => _context.Dispose();
    }
}
