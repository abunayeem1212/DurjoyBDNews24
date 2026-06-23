using DurjoyBDNews24.Domain.Interfaces;
using DurjoyBDNews24.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DurjoyBDNews24.Infrastructure.Repositories;

public class BaseRepository<T>(ApplicationDbContext ctx) : IBaseRepository<T> where T : class
{
    protected readonly ApplicationDbContext _ctx = ctx;
    protected readonly DbSet<T> _db = ctx.Set<T>();

    public async Task<T?> GetByIdAsync(int id) => await _db.FindAsync(id);
    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _db.AsNoTracking().ToListAsync();
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await _db.AsNoTracking().Where(predicate).ToListAsync();
    public async Task<T> AddAsync(T entity)
    {
        await _db.AddAsync(entity);
        return entity;
    }
    public Task UpdateAsync(T entity)
    {
        _db.Update(entity);
        return Task.CompletedTask;
    }
    public Task DeleteAsync(T entity)
    {
        _db.Remove(entity);
        return Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) =>
        await _db.AnyAsync(predicate);
    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null) =>
        predicate == null ? await _db.CountAsync() : await _db.CountAsync(predicate);
}