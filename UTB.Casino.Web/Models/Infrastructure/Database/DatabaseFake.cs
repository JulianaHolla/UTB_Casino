using UTB.Casino.Web.Models.Entities;
using UTB.Casino.Web.Models.Infrastructure.GameLogic;

namespace UTB.Casino.Web.Models.Infrastructure.Database;

/// <summary>In-memory "database". Replace with EF Core / a Class Library later.</summary>
public static class DatabaseFake
{
    public static IList<Player> Players { get; } = new List<Player>();
    public static IList<Game> Games { get; } = new List<Game>();
    public static IList<Bet> Bets { get; } = new List<Bet>();
    public static IList<Transaction> Transactions { get; } = new List<Transaction>();

    /// <summary>Blackjack round in progress (or just finished) for the current player.</summary>
    public static BlackjackRound? ActiveBlackjack { get; set; }

    /// <summary>No login in this simulation: the first player is "the" player.</summary>
    public static Player CurrentPlayer => Players[0];

    private static readonly object Sync = new();

    /// <summary>Changes balance by a signed amount and logs a transaction.</summary>
    public static void Apply(Player player, decimal signedAmount, string type, string note)
    {
        lock (Sync)
        {
            player.Balance += signedAmount;
            Transactions.Add(new Transaction
            {
                Id = Transactions.Count + 1,
                PlayerId = player.Id,
                Amount = signedAmount,
                Type = type,
                Note = note
            });
        }
    }

    /// <summary>Takes a stake. Returns false when the balance is too low.</summary>
    public static bool TryDebit(Player player, decimal amount, string note)
    {
        lock (Sync)
        {
            if (amount <= 0 || player.Balance < amount) return false;
            Apply(player, -amount, "Bet", note);
            return true;
        }
    }

    public static void RecordBet(Player player, Game game, decimal amount, decimal payout, string result)
    {
        lock (Sync)
        {
            Bets.Add(new Bet
            {
                Id = Bets.Count + 1,
                PlayerId = player.Id,
                GameId = game.Id,
                GameName = game.Name,
                Amount = amount,
                Payout = payout,
                Result = result
            });
        }
    }
}
