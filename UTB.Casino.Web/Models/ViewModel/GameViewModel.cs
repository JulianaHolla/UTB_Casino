using UTB.Casino.Web.Models.Entities;

namespace UTB.Casino.Web.Models.ViewModel;

public abstract class GameViewModel
{
    public Player Player { get; set; } = new();
    public Game Game { get; set; } = new();
    public decimal Amount { get; set; }
    public string? Message { get; set; }
    public bool IsError { get; set; }
}
