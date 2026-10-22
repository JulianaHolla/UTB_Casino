using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Application.Dto;
using UTB.Casino.Domain.Constants;
using UTB.Casino.Web.Areas.Admin.Models;

namespace UTB.Casino.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = Roles.AdminOrManager)]
public class GameController : Controller
{
    private readonly IGameService _games;
    private readonly ILogger<GameController> _logger;

    public GameController(IGameService games, ILogger<GameController> logger) { _games = games; _logger = logger; }

    public async Task<IActionResult> Select() => View(await _games.GetAllAsync());

    [HttpGet]
    public IActionResult Create() => View(WithSlugs(new GameEditViewModel()));

    [HttpPost]
    public async Task<IActionResult> Create(GameEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(WithSlugs(vm));

        using var image = vm.Image?.OpenReadStream();
        var result = await _games.CreateAsync(ToDto(vm, image));
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError("", error);
            return View(WithSlugs(vm));
        }
        _logger.LogInformation("Game {Slug} created from admin", vm.Slug);
        return RedirectToAction(nameof(Select));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var game = await _games.GetByIdAsync(id);
        if (game == null) return NotFound();
        return View(WithSlugs(new GameEditViewModel
        {
            Id = game.Id, Name = game.Name, Slug = game.Slug, Description = game.Description,
            MinBet = game.MinBet, MaxBet = game.MaxBet, IsActive = game.IsActive, ImagePath = game.ImagePath
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(GameEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(WithSlugs(vm));

        using var image = vm.Image?.OpenReadStream();
        var result = await _games.UpdateAsync(ToDto(vm, image));
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError("", error);
            return View(WithSlugs(vm));
        }
        _logger.LogInformation("Game {GameId} edited from admin", vm.Id);
        return RedirectToAction(nameof(Select));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _games.DeleteAsync(id);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded ? "Game deleted." : result.Error;
        return RedirectToAction(nameof(Select));
    }

    private static GameSaveDto ToDto(GameEditViewModel vm, Stream? image) => new()
    {
        Id = vm.Id, Name = vm.Name, Slug = vm.Slug, Description = vm.Description,
        MinBet = vm.MinBet, MaxBet = vm.MaxBet, IsActive = vm.IsActive,
        ImageStream = image, ImageFileName = vm.Image?.FileName
    };

    private static GameEditViewModel WithSlugs(GameEditViewModel vm)
    {
        vm.Slugs = GameSlugs.All.Select(s => new SelectListItem(s, s)).ToList();
        return vm;
    }
}
