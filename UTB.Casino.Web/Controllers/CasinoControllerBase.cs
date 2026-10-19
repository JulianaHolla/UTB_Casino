using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Web.Models.Entities;
using UTB.Casino.Web.Models.Infrastructure.Database;

namespace UTB.Casino.Web.Controllers;

[AutoValidateAntiforgeryToken]
public abstract class CasinoControllerBase : Controller
{
    protected Player CurrentPlayer => DatabaseFake.CurrentPlayer;

    protected Game GetGame(string slug) => DatabaseFake.Games.First(g => g.Slug == slug);

    protected bool TryValidateBet(Game game, decimal amount, out string error)
    {
        error = "";
        if (CurrentPlayer.IsBlocked) error = "Your account is blocked.";
        else if (!game.IsActive) error = $"{game.Name} is closed right now.";
        else if (amount < game.MinBet || amount > game.MaxBet)
            error = $"Bet between {game.MinBet:N2} and {game.MaxBet:N2}.";
        else if (amount > CurrentPlayer.Balance) error = "Not enough credits. Add more in your wallet.";
        return error == "";
    }
}
