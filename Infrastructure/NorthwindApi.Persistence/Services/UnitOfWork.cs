using Microsoft.EntityFrameworkCore.Storage;
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
        private IDbContextTransaction? _transaction;

        public UnitOfWork(NorthwindDbContext context) => _context = context;

        public IGenericRepository<T> Repository<T>() where T : class
        {
            if (!_repositories.ContainsKey(typeof(T)))
                _repositories[typeof(T)] = new GenericRepository<T>(_context);

            return (IGenericRepository<T>)_repositories[typeof(T)];
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
            => await _context.SaveChangesAsync(cancellationToken);
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    => _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction!.CommitAsync(cancellationToken);
            _transaction.Dispose(); 
            _transaction = null;   
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _transaction!.RollbackAsync(cancellationToken);
            _transaction.Dispose(); 
            _transaction = null;    

        }

        public void Dispose() { _transaction?.Dispose(); _context.Dispose(); }
    }
}
