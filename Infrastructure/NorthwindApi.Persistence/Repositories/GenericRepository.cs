using Microsoft.EntityFrameworkCore;
using NorthwindApi.Application.Interfaces.Repositories;
using NorthwindApi.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly NorthwindDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(NorthwindDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> GetAll() => _dbSet.AsNoTracking();

        public async Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken)
            => await _dbSet.FindAsync(new[] { id }, cancellationToken);

        public async Task AddAsync(T entity, CancellationToken cancellationToken)
            => await _dbSet.AddAsync(entity, cancellationToken);

        public void Update(T entity) => _dbSet.Update(entity);
        public void Delete(T entity) => _dbSet.Remove(entity);
    }
}
