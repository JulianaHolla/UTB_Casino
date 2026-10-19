using UTB.Casino.Web.Models.Infrastructure.GameLogic;

namespace UTB.Casino.Web.Models.ViewModel;

public class BlackjackViewModel : GameViewModel
{
    public BlackjackRound? Round { get; set; }
    public bool InProgress => Round is { IsFinished: false };
}
