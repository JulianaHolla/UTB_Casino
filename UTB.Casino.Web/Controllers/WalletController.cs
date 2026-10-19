using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Web.Models.Infrastructure.Database;
using UTB.Casino.Web.Models.ViewModel;

namespace UTB.Casino.Web.Controllers;

public class WalletController : CasinoControllerBase
{
    private const decimal MaxDeposit = 1000m;

    public IActionResult Index() => View(new WalletViewModel
    {
        Player = CurrentPlayer,
        Transactions = DatabaseFake.Transactions.Where(t => t.PlayerId == CurrentPlayer.Id)
                                                .OrderByDescending(t => t.CreatedAt).Take(20).ToList()
    });

    [HttpGet]
    public IActionResult Deposit() => View(new BetViewModel { Amount = 100 });

    [HttpPost]
    public IActionResult Deposit(BetViewModel deposit)
    {
        var amount = Math.Round(deposit.Amount, 2);
        if (CurrentPlayer.IsBlocked)
            ModelState.AddModelError("", "Your account is blocked.");
        else if (amount < 1 || amount > MaxDeposit)
            ModelState.AddModelError("", $"Add between 1 and {MaxDeposit:N0} credits at a time.");

        if (!ModelState.IsValid) return View(deposit);

        DatabaseFake.Apply(CurrentPlayer, amount, "Deposit", "Virtual credits");
        TempData["Success"] = $"Added {amount:N2} credits.";
        return RedirectToAction(nameof(Index));
    }
}
