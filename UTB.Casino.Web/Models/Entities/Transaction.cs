namespace UTB.Casino.Web.Models.Entities;

public class Transaction
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    /// <summary>Signed: negative = money out, positive = money in.</summary>
    public decimal Amount { get; set; }
    /// <summary>Deposit, Bet, Win or Adjustment.</summary>
    public string Type { get; set; } = "";
    public string Note { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
