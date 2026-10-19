namespace UTB.Casino.Web.Models.ViewModel;

public class RouletteViewModel : GameViewModel
{
    public string BetType { get; set; } = "red";
    public int? Number { get; set; }
    public int? Result { get; set; }
    public string ResultColor { get; set; } = "";
    public decimal Payout { get; set; }
}
