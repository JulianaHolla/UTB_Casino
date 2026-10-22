using Microsoft.Extensions.Logging;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Application.Dto;
using UTB.Casino.Application.GameLogic;
using UTB.Casino.Domain.Constants;

namespace UTB.Casino.Application.Services;

public class RouletteService : GameServiceBase, IRouletteService
{
    private readonly ILogger<RouletteService> _logger;

    public RouletteService(IUserRepository users, IGameRepository games, IBetRepository bets,
                           IWalletTransactionRepository tx, IUnitOfWork uow, ILogger<RouletteService> logger)
        : base(users, games, bets, tx, uow) => _logger = logger;

    public async Task<Result<RouletteResultDto>> SpinAsync(string userId, decimal amount, string betType, int? number)
    {
        if (!RouletteWheel.IsValidBet(betType, number))
            return Result<RouletteResultDto>.Fail("Pick a colour, odd/even, a half, or a number from 0 to 36.");

        amount = Math.Round(amount, 2);
        var (user, game, error) = await PrepareAsync(userId, GameSlugs.Roulette, amount);
        if (error != null) return Result<RouletteResultDto>.Fail(error);

        var spin = RouletteWheel.Spin();
        var color = RouletteWheel.ColorOf(spin);
        var payout = Math.Round(amount * RouletteWheel.Multiplier(betType, number, spin), 2);
        await SettleAsync(user!, game!, amount, payout, $"{spin} {color} (bet: {betType})", false);

        _logger.LogInformation("Roulette: user {UserId} bet {Amount} on {BetType}, spin {Spin}, payout {Payout}", userId, amount, betType, spin, payout);
        return Result<RouletteResultDto>.Ok(new RouletteResultDto(spin, color, payout));
    }
}
