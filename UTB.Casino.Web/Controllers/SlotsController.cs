using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Web.Models.Infrastructure.Database;
using UTB.Casino.Web.Models.Infrastructure.GameLogic;
using UTB.Casino.Web.Models.ViewModel;

namespace UTB.Casino.Web.Controllers;

public class SlotsController : CasinoControllerBase
{
    private const string Slug = "Slots";

    [HttpGet]
    public IActionResult Play() => View(BuildViewModel(0));

    [HttpPost]
    public IActionResult Play(BetViewModel bet)
    {
        var amount = Math.Round(bet.Amount, 2);
        var game = GetGame(Slug);
        var vm = BuildViewModel(amount);

        if (!TryValidateBet(game, amount, out var error))
        {
            vm.Message = error;
            vm.IsError = true;
            return View(vm);
        }

        DatabaseFake.TryDebit(CurrentPlayer, amount, game.Name);
        var spin = SlotMachine.Spin();
        var payout = Math.Round(amount * spin.Multiplier, 2);
        if (payout > 0) DatabaseFake.Apply(CurrentPlayer, payout, "Win", game.Name);
        DatabaseFake.RecordBet(CurrentPlayer, game, amount, payout, spin.Description);

        vm.Reels = spin.Reels;
        vm.HasResult = true;
        vm.Payout = payout;
        vm.Message = payout > 0 ? $"{spin.Description}. You won {payout:N2}." : "No match. Spin again.";
        return View(vm);
    }

    private SlotsViewModel BuildViewModel(decimal amount)
    {
        var game = GetGame(Slug);
        return new SlotsViewModel { Player = CurrentPlayer, Game = game, Amount = amount > 0 ? amount : game.MinBet };
    }
}
