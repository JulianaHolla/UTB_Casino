namespace UTB.Casino.Web.Models.Entities;

public class Bet
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public int GameId { get; set; }
    public string GameName { get; set; } = "";
    public decimal Amount { get; set; }
    /// <summary>Total returned to the player (stake included). 0 = lost.</summary>
    public decimal Payout { get; set; }
    public string Result { get; set; } = "";
    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
}
