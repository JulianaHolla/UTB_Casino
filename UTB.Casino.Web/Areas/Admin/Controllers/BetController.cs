using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Domain.Constants;
using UTB.Casino.Web.Areas.Admin.Models;

namespace UTB.Casino.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.AdminOrManager)]
public class BetController : Controller
{
    private readonly IBetService _bets;
    private readonly ILogger<BetController> _logger;

    public BetController(IBetService bets, ILogger<BetController> logger) { _bets = bets; _logger = logger; }

    public async Task<IActionResult> Select() => View(await _bets.GetAllAsync(200));

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var bet = await _bets.GetByIdAsync(id);
        if (bet == null) return NotFound();
        return View(new BetEditViewModel
        {
            Id = bet.Id, Result = bet.Result,
            Summary = $"{bet.User?.UserName} on {bet.Game?.Name}: bet {bet.Amount:N2}, returned {bet.Payout:N2}"
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(BetEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var result = await _bets.UpdateResultAsync(vm.Id, vm.Result);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", result.Error);
            return View(vm);
        }
        _logger.LogInformation("Bet {BetId} edited from admin", vm.Id);
        return RedirectToAction(nameof(Select));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _bets.DeleteAsync(id);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded ? "Bet deleted." : result.Error;
        return RedirectToAction(nameof(Select));
    }
}
