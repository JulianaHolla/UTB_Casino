using Microsoft.EntityFrameworkCore;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Domain.Entities;
using UTB.Casino.Infrastructure.Data;

namespace UTB.Casino.Infrastructure.Repositories;

public class WalletTransactionRepository : EfRepository<WalletTransaction>, IWalletTransactionRepository
{
    public WalletTransactionRepository(CasinoDbContext context) : base(context) { }

    public override Task<WalletTransaction?> GetByIdAsync(int id) =>
        Set.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);

    public override async Task<IList<WalletTransaction>> GetAllAsync() => await GetRecentAsync(int.MaxValue);

    public async Task<IList<WalletTransaction>> GetByUserAsync(string userId, int take) =>
        await Set.Where(t => t.UserId == userId).OrderByDescending(t => t.CreatedAt).Take(take).ToListAsync();

    public async Task<IList<WalletTransaction>> GetRecentAsync(int take) =>
        await Set.Include(t => t.User).OrderByDescending(t => t.CreatedAt).Take(take).ToListAsync();
}
