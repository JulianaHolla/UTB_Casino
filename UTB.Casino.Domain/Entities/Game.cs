namespace UTB.Casino.Domain.Entities;

public class Game
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal MinBet { get; set; }
    public decimal MaxBet { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ImagePath { get; set; }

    public ICollection<Bet> Bets { get; set; } = new List<Bet>();
}
