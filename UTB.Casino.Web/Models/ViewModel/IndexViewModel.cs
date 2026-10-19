using UTB.Casino.Web.Models.Entities;

namespace UTB.Casino.Web.Models.ViewModel;

public class IndexViewModel
{
    public Player Player { get; set; } = new();
    public IList<Game> Games { get; set; } = new List<Game>();
    public IList<Bet> RecentBets { get; set; } = new List<Bet>();
}
