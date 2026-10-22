using Microsoft.Extensions.Logging;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Application.Dto;
using UTB.Casino.Domain.Entities;

namespace UTB.Casino.Application.Services;

public class BetService : IBetService
{
    private readonly IBetRepository _bets;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<BetService> _logger;

    public BetService(IBetRepository bets, IUnitOfWork uow, ILogger<BetService> logger)
    { _bets = bets; _uow = uow; _logger = logger; }

    public Task<IList<Bet>> GetUserBetsAsync(string userId, int take) => _bets.GetByUserAsync(userId, take);
    public Task<IList<Bet>> GetAllAsync(int take) => _bets.GetRecentAsync(take);
    public Task<Bet?> GetByIdAsync(int id) => _bets.GetByIdAsync(id);

    public async Task<Result> UpdateResultAsync(int id, string result)
    {
        var bet = await _bets.GetByIdAsync(id);
        if (bet == null) return Result.Fail("Bet not found.");
        if (string.IsNullOrWhiteSpace(result)) return Result.Fail("Result text is required.");
        bet.Result = result.Trim();
        _bets.Update(bet);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var bet = await _bets.GetByIdAsync(id);
        if (bet == null) return Result.Fail("Bet not found.");
        _bets.Remove(bet);   // ledger rows keep existing: FK is SET NULL
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Bet {BetId} deleted", id);
        return Result.Ok();
    }
}
