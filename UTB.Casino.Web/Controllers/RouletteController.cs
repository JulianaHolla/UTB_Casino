using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Domain.Constants;
using UTB.Casino.Web.Models.ViewModel;

namespace UTB.Casino.Web.Controllers;

public class RouletteController : CasinoControllerBase
{
    private readonly IRouletteService _roulette;
    private readonly IGameService _games;
    private readonly ILogger<RouletteController> _logger;

    public RouletteController(IRouletteService roulette, IGameService games, ILogger<RouletteController> logger)
    { _roulette = roulette; _games = games; _logger = logger; }

    [HttpGet]
    public async Task<IActionResult> Play() => View(await BuildAsync(new BetFormViewModel()));

    [HttpPost]
    public async Task<IActionResult> Play(BetFormViewModel bet)
    {
        var vm = await BuildAsync(bet);
        if (!ModelState.IsValid) return View(vm);

        var result = await _roulette.SpinAsync(UserId, bet.Amount, bet.BetType, bet.Number);
        if (!result.Succeeded)
        {
            vm.Message = result.Error;
            vm.IsError = true;
            return View(vm);
        }

        var spin = result.Value!;
        _logger.LogInformation("Roulette played by {UserId}: {Number} {Color}", UserId, spin.Number, spin.Color);
        vm.Result = spin;
        vm.Message = spin.Payout > 0 ? $"{spin.Number} {spin.Color}. You won {spin.Payout:N2}." : $"{spin.Number} {spin.Color}. No win this time.";
        return View(vm);
    }

    private async Task<RouletteViewModel> BuildAsync(BetFormViewModel bet)
    {
        var game = (await _games.GetBySlugAsync(GameSlugs.Roulette))!;
        return new RouletteViewModel { Game = game, Amount = bet.Amount > 0 ? bet.Amount : game.MinBet, BetType = bet.BetType, Number = bet.Number };
    }
}
