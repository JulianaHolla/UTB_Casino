using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Domain.Constants;
using UTB.Casino.Web.Models.ViewModel;

namespace UTB.Casino.Web.Controllers;

public class SlotsController : CasinoControllerBase
{
    private readonly ISlotsService _slots;
    private readonly IGameService _games;
    private readonly ILogger<SlotsController> _logger;

    public SlotsController(ISlotsService slots, IGameService games, ILogger<SlotsController> logger)
    { _slots = slots; _games = games; _logger = logger; }

    [HttpGet]
    public async Task<IActionResult> Play() => View(await BuildAsync(0));

    [HttpPost]
    public async Task<IActionResult> Play(BetFormViewModel bet)
    {
        var vm = await BuildAsync(bet.Amount);
        if (!ModelState.IsValid) return View(vm);

        var result = await _slots.SpinAsync(UserId, bet.Amount);
        if (!result.Succeeded)
        {
            vm.Message = result.Error;
            vm.IsError = true;
            return View(vm);
        }

        var spin = result.Value!;
        _logger.LogInformation("Slots played by {UserId}: {Description}", UserId, spin.Description);
        vm.Reels = spin.Reels;
        vm.HasResult = true;
        vm.Payout = spin.Payout;
        vm.Message = spin.Payout > 0 ? $"{spin.Description}. You won {spin.Payout:N2}." : "No match. Spin again.";
        return View(vm);
    }

    private async Task<SlotsViewModel> BuildAsync(decimal amount)
    {
        var game = (await _games.GetBySlugAsync(GameSlugs.Slots))!;
        return new SlotsViewModel { Game = game, Amount = amount > 0 ? amount : game.MinBet };
    }
}
