using Microsoft.EntityFrameworkCore;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Infrastructure.Data;

namespace UTB.Casino.Infrastructure.Repositories;

public class EfRepository<T> : IRepository<T> where T : class
{
    protected readonly CasinoDbContext Context;
    protected DbSet<T> Set => Context.Set<T>();

    public EfRepository(CasinoDbContext context) => Context = context;

    public virtual async Task<T?> GetByIdAsync(int id) => await Set.FindAsync(id);
    public virtual async Task<IList<T>> GetAllAsync() => await Set.ToListAsync();
    public virtual async Task AddAsync(T entity) => await Set.AddAsync(entity);
    public virtual void Update(T entity) => Set.Update(entity);
    public virtual void Remove(T entity) => Set.Remove(entity);
}
