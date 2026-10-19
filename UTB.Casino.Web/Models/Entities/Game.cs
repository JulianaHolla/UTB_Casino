namespace UTB.Casino.Web.Models.Entities;

public class Game
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    /// <summary>Must match the controller name (Slots, Roulette, Blackjack).</summary>
    public string Slug { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal MinBet { get; set; }
    public decimal MaxBet { get; set; }
    public bool IsActive { get; set; } = true;
}
