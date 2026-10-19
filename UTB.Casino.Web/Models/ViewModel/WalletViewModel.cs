using UTB.Casino.Web.Models.Entities;

namespace UTB.Casino.Web.Models.ViewModel;

public class WalletViewModel
{
    public Player Player { get; set; } = new();
    public IList<Transaction> Transactions { get; set; } = new List<Transaction>();
}
