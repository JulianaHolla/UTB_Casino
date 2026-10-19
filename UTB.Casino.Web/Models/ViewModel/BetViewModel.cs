namespace UTB.Casino.Web.Models.ViewModel;

/// <summary>Posted by every bet form. BetType/Number are only used by roulette.</summary>
public class BetViewModel
{
    public decimal Amount { get; set; }
    public string BetType { get; set; } = "red";
    public int? Number { get; set; }
}
