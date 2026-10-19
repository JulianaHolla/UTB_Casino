using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Web.Models.Entities;
using UTB.Casino.Web.Models.Infrastructure.Database;
using UTB.Casino.Web.Models.Infrastructure.GameLogic;
using UTB.Casino.Web.Models.ViewModel;

namespace UTB.Casino.Web.Controllers;

public class BlackjackController : CasinoControllerBase
{
    private const string Slug = "Blackjack";

    [HttpGet]
    public IActionResult Play()
    {
        var game = GetGame(Slug);
        var round = DatabaseFake.ActiveBlackjack;
        var vm = new BlackjackViewModel
        {
            Player = CurrentPlayer, Game = game, Round = round,
            Amount = round?.Bet ?? game.MinBet,
            Message = TempData["Error"] as string,
            IsError = TempData["Error"] != null
        };

        if (round is { IsFinished: true })
        {
            var payout = BlackjackEngine.Payout(round);
            vm.Message = round.State switch
            {
                RoundState.PlayerBlackjack => $"Blackjack! You won {payout:N2}.",
                RoundState.PlayerWin => $"You win {payout:N2}.",
                RoundState.Push => "Push. Your bet is returned.",
                _ => "Dealer wins."
            };
        }
        return View(vm);
    }

    [HttpPost]
    public IActionResult Deal(BetViewModel bet)
    {
        var amount = Math.Round(bet.Amount, 2);
        var game = GetGame(Slug);

        if (DatabaseFake.ActiveBlackjack is { IsFinished: false }) return RedirectToAction(nameof(Play));
        if (!TryValidateBet(game, amount, out var error))
        {
            TempData["Error"] = error;
            return RedirectToAction(nameof(Play));
        }

        DatabaseFake.TryDebit(CurrentPlayer, amount, game.Name);
        var round = BlackjackEngine.Start(amount);
        DatabaseFake.ActiveBlackjack = round;
        if (round.IsFinished) Settle(round, game);
        return RedirectToAction(nameof(Play));
    }

    [HttpPost]
    public IActionResult Hit()
    {
        if (DatabaseFake.ActiveBlackjack is { IsFinished: false } round)
        {
            BlackjackEngine.Hit(round);
            if (round.IsFinished) Settle(round, GetGame(Slug));
        }
        return RedirectToAction(nameof(Play));
    }

    [HttpPost]
    public IActionResult Stand()
    {
        if (DatabaseFake.ActiveBlackjack is { IsFinished: false } round)
        {
            BlackjackEngine.Stand(round);
            Settle(round, GetGame(Slug));
        }
        return RedirectToAction(nameof(Play));
    }

    private void Settle(BlackjackRound round, Game game)
    {
        var payout = BlackjackEngine.Payout(round);
        if (payout > 0) DatabaseFake.Apply(CurrentPlayer, payout, "Win", game.Name);
        DatabaseFake.RecordBet(CurrentPlayer, game, round.Bet, payout, round.State.ToString());
    }
}
