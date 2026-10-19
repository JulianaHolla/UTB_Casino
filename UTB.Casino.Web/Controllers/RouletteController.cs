using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Web.Models.Infrastructure.Database;
using UTB.Casino.Web.Models.Infrastructure.GameLogic;
using UTB.Casino.Web.Models.ViewModel;

namespace UTB.Casino.Web.Controllers;

public class RouletteController : CasinoControllerBase
{
    private const string Slug = "Roulette";

    [HttpGet]
    public IActionResult Play() => View(BuildViewModel(new BetViewModel()));

    [HttpPost]
    public IActionResult Play(BetViewModel bet)
    {
        bet.Amount = Math.Round(bet.Amount, 2);
        var game = GetGame(Slug);
        var vm = BuildViewModel(bet);

        if (!RouletteWheel.IsValidBet(bet.BetType, bet.Number))
        {
            vm.Message = "Pick a colour, odd/even, a half, or a number from 0 to 36.";
            vm.IsError = true;
            return View(vm);
        }
        if (!TryValidateBet(game, bet.Amount, out var error))
        {
            vm.Message = error;
            vm.IsError = true;
            return View(vm);
        }

        DatabaseFake.TryDebit(CurrentPlayer, bet.Amount, game.Name);
        var spin = RouletteWheel.Spin();
        var payout = Math.Round(bet.Amount * RouletteWheel.Multiplier(bet.BetType, bet.Number, spin), 2);
        if (payout > 0) DatabaseFake.Apply(CurrentPlayer, payout, "Win", game.Name);
        var color = RouletteWheel.ColorOf(spin);
        DatabaseFake.RecordBet(CurrentPlayer, game, bet.Amount, payout, $"{spin} {color} (bet: {bet.BetType})");

        vm.Result = spin;
        vm.ResultColor = color;
        vm.Payout = payout;
        vm.Message = payout > 0 ? $"{spin} {color}. You won {payout:N2}." : $"{spin} {color}. No win this time.";
        return View(vm);
    }

    private RouletteViewModel BuildViewModel(BetViewModel bet)
    {
        var game = GetGame(Slug);
        return new RouletteViewModel
        {
            Player = CurrentPlayer, Game = game,
            Amount = bet.Amount > 0 ? bet.Amount : game.MinBet,
            BetType = bet.BetType, Number = bet.Number
        };
    }
}
