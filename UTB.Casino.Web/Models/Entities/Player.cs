namespace UTB.Casino.Web.Models.Entities;

public class Player
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public decimal Balance { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
