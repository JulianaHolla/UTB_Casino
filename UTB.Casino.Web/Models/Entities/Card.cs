namespace UTB.Casino.Web.Models.Entities;

public class Card
{
    public string Suit { get; set; } = "";
    public string Rank { get; set; } = "";
    /// <summary>Ace counts as 11 here; the engine lowers it when needed.</summary>
    public int Value { get; set; }
    public bool IsRed => Suit is "♥" or "♦";
}
