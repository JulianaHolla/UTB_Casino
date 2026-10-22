using UTB.Casino.Application.Abstraction;
using UTB.Casino.Domain.Entities;
using UTB.Casino.Domain.Enums;

namespace UTB.Casino.Application.Services;

/// <summary>Shared rules for every casino game: validation, stake, settlement.</summary>
public abstract class GameServiceBase
{
    protected readonly IUserRepository Users;
    protected readonly IGameRepository Games;
    protected readonly IBetRepository Bets;
    protected readonly IWalletTransactionRepository Transactions;
    protected readonly IUnitOfWork Uow;

    protected GameServiceBase(IUserRepository users, IGameRepository games, IBetRepository bets,
                              IWalletTransactionRepository transactions, IUnitOfWork uow)
    {
        Users = users; Games = games; Bets = bets; Transactions = transactions; Uow = uow;
    }

    protected async Task<(ApplicationUser? User, Game? Game, string? Error)> PrepareAsync(string userId, string slug, decimal amount)
    {
        var user = await Users.GetByIdAsync(userId);
        if (user == null) return (null, null, "Player not found.");
        var game = await Games.GetBySlugAsync(slug);
        if (game == null) return (user, null, "Game not found.");

        string? error = null;
        if (user.IsBlocked) error = "Your account is blocked.";
        else if (!game.IsActive) error = $"{game.Name} is closed right now.";
        else if (amount < game.MinBet || amount > game.MaxBet) error = $"Bet between {game.MinBet:N2} and {game.MaxBet:N2}.";
        else if (amount > user.Balance) error = "Not enough credits. Add more in your wallet.";
        return (user, game, error);
    }

    /// <summary>Writes the bet, the ledger rows and the new balance in one SaveChanges (atomic).</summary>
    protected async Task SettleAsync(ApplicationUser user, Game game, decimal amount, decimal payout, string result, bool stakeAlreadyTaken)
    {
        var bet = new Bet { UserId = user.Id, GameId = game.Id, Amount = amount, Payout = payout, Result = result };
        await Bets.AddAsync(bet);

        if (!stakeAlreadyTaken)
        {
            user.Balance -= amount;
            await Transactions.AddAsync(new WalletTransaction { UserId = user.Id, Bet = bet, Amount = -amount, Type = TransactionType.Stake, Note = game.Name });
        }
        if (payout > 0)
        {
            user.Balance += payout;
            await Transactions.AddAsync(new WalletTransaction { UserId = user.Id, Bet = bet, Amount = payout, Type = TransactionType.Win, Note = game.Name });
        }
        await Uow.SaveChangesAsync();
    }
}
