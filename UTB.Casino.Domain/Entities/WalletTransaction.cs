using UTB.Casino.Domain.Enums;

namespace UTB.Casino.Domain.Entities;

public class WalletTransaction
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public int? BetId { get; set; }
    /// <summary>Signed: negative = credits out, positive = credits in.</summary>
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string Note { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser? User { get; set; }
    public Bet? Bet { get; set; }
}
