using Microsoft.EntityFrameworkCore;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Domain.Entities;
using UTB.Casino.Infrastructure.Data;

namespace UTB.Casino.Infrastructure.Repositories;

public class BetRepository : EfRepository<Bet>, IBetRepository
{
    public BetRepository(CasinoDbContext context) : base(context) { }

    public override Task<Bet?> GetByIdAsync(int id) =>
        Set.Include(b => b.User).Include(b => b.Game).FirstOrDefaultAsync(b => b.Id == id);

    public override async Task<IList<Bet>> GetAllAsync() => await GetRecentAsync(int.MaxValue);

    public async Task<IList<Bet>> GetByUserAsync(string userId, int take) =>
        await Set.Include(b => b.Game).Where(b => b.UserId == userId).OrderByDescending(b => b.PlacedAt).Take(take).ToListAsync();

    public async Task<IList<Bet>> GetRecentAsync(int take) =>
        await Set.Include(b => b.User).Include(b => b.Game).OrderByDescending(b => b.PlacedAt).Take(take).ToListAsync();
}
