using Microsoft.EntityFrameworkCore;
using OnlineFruitShop.Core.Interfaces;
using OnlineFruitShop.Infrastructure.Data;

namespace OnlineFruitShop.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _entities;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public async Task AddAsync(T entity) => await _entities.AddAsync(entity);

        public async Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate) =>
            await _entities.Where(predicate).ToListAsync();

        public async Task<IEnumerable<T>> GetAllAsync() => await _entities.ToListAsync();

        public async Task<T?> GetByIdAsync(int id) => await _entities.FindAsync(id);

        public void Remove(T entity) => _entities.Remove(entity);

        public void Update(T entity) => _entities.Update(entity);

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
