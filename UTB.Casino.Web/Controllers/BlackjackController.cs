using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Domain.Constants;
using UTB.Casino.Web.Models.ViewModel;

namespace UTB.Casino.Web.Controllers;

public class BlackjackController : CasinoControllerBase
{
    private readonly IBlackjackService _blackjack;
    private readonly IGameService _games;
    private readonly ILogger<BlackjackController> _logger;

    public BlackjackController(IBlackjackService blackjack, IGameService games, ILogger<BlackjackController> logger)
    { _blackjack = blackjack; _games = games; _logger = logger; }

    [HttpGet]
    public async Task<IActionResult> Play()
    {
        var game = (await _games.GetBySlugAsync(GameSlugs.Blackjack))!;
        var state = _blackjack.GetState(UserId);
        var vm = new BlackjackViewModel
        {
            Game = game, State = state, Amount = state?.Bet ?? game.MinBet,
            Message = TempData["Error"] as string, IsError = TempData["Error"] != null
        };

        if (state is { IsFinished: true })
            vm.Message = state.Outcome switch
            {
                "PlayerBlackjack" => $"Blackjack! You won {state.Payout:N2}.",
                "PlayerWin" => $"You win {state.Payout:N2}.",
                "Push" => "Push. Your bet is returned.",
                _ => "Dealer wins."
            };
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Deal(BetFormViewModel bet)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Enter a valid bet amount.";
            return RedirectToAction(nameof(Play));
        }
        var result = await _blackjack.DealAsync(UserId, bet.Amount);
        if (!result.Succeeded) TempData["Error"] = result.Error;
        return RedirectToAction(nameof(Play));
    }

    [HttpPost]
    public async Task<IActionResult> Hit()
    {
        var result = await _blackjack.HitAsync(UserId);
        if (!result.Succeeded) TempData["Error"] = result.Error;
        return RedirectToAction(nameof(Play));
    }

    [HttpPost]
    public async Task<IActionResult> Stand()
    {
        var result = await _blackjack.StandAsync(UserId);
        if (!result.Succeeded) TempData["Error"] = result.Error;
        return RedirectToAction(nameof(Play));
    }
}
