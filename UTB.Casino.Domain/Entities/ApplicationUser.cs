using Microsoft.AspNetCore.Identity;

namespace UTB.Casino.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public decimal Balance { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    public ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
}
