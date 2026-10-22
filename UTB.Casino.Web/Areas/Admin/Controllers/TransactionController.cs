using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Domain.Constants;
using UTB.Casino.Web.Areas.Admin.Models;

namespace UTB.Casino.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.AdminOrManager)]
public class TransactionController : Controller
{
    private readonly IWalletService _wallet;
    public TransactionController(IWalletService wallet) => _wallet = wallet;

    public async Task<IActionResult> Select() => View(await _wallet.GetAllTransactionsAsync(200));

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var tx = await _wallet.GetTransactionAsync(id);
        if (tx == null) return NotFound();
        return View(new TransactionEditViewModel
        {
            Id = tx.Id, Note = tx.Note,
            Summary = $"{tx.User?.UserName}: {tx.Type} {tx.Amount:+0.00;-0.00}"
        });
    }

    [HttpPost]
    public async Task<IActionResult> Edit(TransactionEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        var result = await _wallet.UpdateTransactionNoteAsync(vm.Id, vm.Note);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", result.Error);
            return View(vm);
        }
        return RedirectToAction(nameof(Select));
    }
}
