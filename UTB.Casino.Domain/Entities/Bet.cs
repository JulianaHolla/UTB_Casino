namespace UTB.Casino.Domain.Entities;

public class Bet
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public int GameId { get; set; }
    public decimal Amount { get; set; }
    /// <summary>Total returned to the player (stake included). 0 = lost.</summary>
    public decimal Payout { get; set; }
    public string Result { get; set; } = "";
    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser? User { get; set; }
    public Game? Game { get; set; }
    public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
}
