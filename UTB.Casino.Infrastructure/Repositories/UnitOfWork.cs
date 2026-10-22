using Microsoft.EntityFrameworkCore;
using UTB.Casino.Application.Abstraction;

namespace UTB.Casino.Infrastructure.Repositories;

/// <summary>Generic over the DbContext, so another context can get its own unit of work.</summary>
public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    private readonly TContext _context;
    public UnitOfWork(TContext context) => _context = context;

    public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();

    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();
        try
        {
            await action();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}
