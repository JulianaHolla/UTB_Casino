using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Web.Models.ViewModel;

namespace UTB.Casino.Web.Controllers;

public class WalletController : CasinoControllerBase
{
    private readonly IWalletService _wallet;
    private readonly ILogger<WalletController> _logger;

    public WalletController(IWalletService wallet, ILogger<WalletController> logger) { _wallet = wallet; _logger = logger; }

    public async Task<IActionResult> Index() => View(new WalletViewModel
    {
        Balance = await _wallet.GetBalanceAsync(UserId),
        Transactions = await _wallet.GetUserTransactionsAsync(UserId, 20)
    });

    [HttpGet]
    public IActionResult Deposit() => View(new DepositViewModel());

    [HttpPost]
    public async Task<IActionResult> Deposit(DepositViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _wallet.DepositAsync(UserId, vm.Amount);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError("", error);
            return View(vm);
        }
        TempData["Success"] = $"Added {vm.Amount:N2} credits.";
        return RedirectToAction(nameof(Index));
    }
}
