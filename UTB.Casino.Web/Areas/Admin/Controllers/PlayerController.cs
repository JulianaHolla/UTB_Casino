using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Web.Models.Infrastructure.Database;

namespace UTB.Casino.Web.Areas.Admin.Controllers;

[Area("Admin")]
[AutoValidateAntiforgeryToken]
public class PlayerController : Controller
{
    public IActionResult Select() => View(DatabaseFake.Players);

    public IActionResult Detail(int id)
    {
        var player = DatabaseFake.Players.FirstOrDefault(p => p.Id == id);
        if (player == null) return NotFound();
        ViewBag.Transactions = DatabaseFake.Transactions.Where(t => t.PlayerId == id)
                                           .OrderByDescending(t => t.CreatedAt).Take(10).ToList();
        return View(player);
    }

    [HttpPost]
    public IActionResult ToggleBlock(int id)
    {
        var player = DatabaseFake.Players.FirstOrDefault(p => p.Id == id);
        if (player == null) return NotFound();
        player.IsBlocked = !player.IsBlocked;
        return RedirectToAction(nameof(Detail), new { id });
    }

    [HttpPost]
    public IActionResult Adjust(int id, decimal amount, string? note)
    {
        var player = DatabaseFake.Players.FirstOrDefault(p => p.Id == id);
        if (player == null) return NotFound();

        amount = Math.Round(amount, 2);
        if (amount == 0 || player.Balance + amount < 0)
            TempData["Error"] = "Amount must not be 0 and balance cannot go below 0.";
        else
            DatabaseFake.Apply(player, amount, "Adjustment", string.IsNullOrWhiteSpace(note) ? "Admin adjustment" : note.Trim());
        return RedirectToAction(nameof(Detail), new { id });
    }
}
