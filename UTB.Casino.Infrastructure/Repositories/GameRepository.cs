using Microsoft.EntityFrameworkCore;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Domain.Entities;
using UTB.Casino.Infrastructure.Data;

namespace UTB.Casino.Infrastructure.Repositories;

public class GameRepository : EfRepository<Game>, IGameRepository
{
    public GameRepository(CasinoDbContext context) : base(context) { }

    public override async Task<IList<Game>> GetAllAsync() => await Set.OrderBy(g => g.Name).ToListAsync();
    public Task<Game?> GetBySlugAsync(string slug) => Set.FirstOrDefaultAsync(g => g.Slug == slug);
    public Task<bool> HasBetsAsync(int gameId) => Context.Bets.AnyAsync(b => b.GameId == gameId);
}
