namespace UTB.Casino.Web.Models.ViewModel;

public class SlotsViewModel : GameViewModel
{
    public string[] Reels { get; set; } = { "🍒", "🍋", "🔔" };
    public bool HasResult { get; set; }
    public decimal Payout { get; set; }
}
