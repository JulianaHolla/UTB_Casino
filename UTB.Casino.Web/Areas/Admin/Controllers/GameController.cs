using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Web.Models.Entities;
using UTB.Casino.Web.Models.Infrastructure.Database;

namespace UTB.Casino.Web.Areas.Admin.Controllers;

[Area("Admin")]
[AutoValidateAntiforgeryToken]
public class GameController : Controller
{
    public IActionResult Select() => View(DatabaseFake.Games);

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var game = DatabaseFake.Games.FirstOrDefault(g => g.Id == id);
        return game == null ? NotFound() : View(game);
    }

    [HttpPost]
    public IActionResult Edit(Game game)
    {
        var existing = DatabaseFake.Games.FirstOrDefault(g => g.Id == game.Id);
        if (existing == null) return NotFound();

        if (string.IsNullOrWhiteSpace(game.Name))
            ModelState.AddModelError("", "Name is required.");
        if (game.MinBet <= 0 || game.MaxBet < game.MinBet)
            ModelState.AddModelError("", "Min bet must be above 0 and not higher than max bet.");
        if (!ModelState.IsValid)
        {
            game.Slug = existing.Slug;
            return View(game);
        }

        existing.Name = game.Name.Trim();
        existing.Description = game.Description ?? "";
        existing.MinBet = game.MinBet;
        existing.MaxBet = game.MaxBet;
        existing.IsActive = game.IsActive;
        return RedirectToAction(nameof(Select));
    }
}
