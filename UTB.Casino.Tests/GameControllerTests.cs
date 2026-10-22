using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Application.Dto;
using UTB.Casino.Domain.Entities;
using UTB.Casino.Web.Areas.Admin.Controllers;
using UTB.Casino.Web.Areas.Admin.Models;

namespace UTB.Casino.Tests;

/// <summary>Admin GameController tests: the controller talks to the database through IGameService (mocked here).</summary>
public class GameControllerTests
{
    private readonly Mock<IGameService> _service = new();
    private readonly GameController _controller;

    public GameControllerTests()
    {
        _controller = new GameController(_service.Object, NullLogger<GameController>.Instance)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
        };
    }

    private static GameEditViewModel ValidVm() => new() { Name = "Slots", Slug = "Slots", MinBet = 1, MaxBet = 100 };

    [Fact]
    public async Task Select_returns_all_games_from_service()
    {
        var games = new List<Game> { new() { Id = 1, Name = "A" }, new() { Id = 2, Name = "B" } };
        _service.Setup(s => s.GetAllAsync()).ReturnsAsync(games);

        var result = await _controller.Select();

        var view = Assert.IsType<ViewResult>(result);
        Assert.Equal(2, Assert.IsAssignableFrom<IList<Game>>(view.Model).Count);
        _service.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task Create_with_invalid_model_does_not_call_service()
    {
        _controller.ModelState.AddModelError("Name", "Required");

        var result = await _controller.Create(ValidVm());

        Assert.IsType<ViewResult>(result);
        _service.Verify(s => s.CreateAsync(It.IsAny<GameSaveDto>()), Times.Never);
    }

    [Fact]
    public async Task Create_valid_calls_service_and_redirects_to_Select()
    {
        _service.Setup(s => s.CreateAsync(It.IsAny<GameSaveDto>())).ReturnsAsync(Result.Ok());

        var result = await _controller.Create(ValidVm());

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(GameController.Select), redirect.ActionName);
        _service.Verify(s => s.CreateAsync(It.Is<GameSaveDto>(d => d.Slug == "Slots" && d.MaxBet == 100)), Times.Once);
    }

    [Fact]
    public async Task Create_service_failure_shows_error_on_form()
    {
        _service.Setup(s => s.CreateAsync(It.IsAny<GameSaveDto>())).ReturnsAsync(Result.Fail("A game of this type already exists."));

        var result = await _controller.Create(ValidVm());

        Assert.IsType<ViewResult>(result);
        Assert.False(_controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Edit_unknown_game_returns_NotFound()
    {
        _service.Setup(s => s.GetByIdAsync(42)).ReturnsAsync((Game?)null);

        Assert.IsType<NotFoundResult>(await _controller.Edit(42));
    }

    [Fact]
    public async Task Edit_valid_updates_through_service()
    {
        _service.Setup(s => s.UpdateAsync(It.IsAny<GameSaveDto>())).ReturnsAsync(Result.Ok());
        var vm = ValidVm();
        vm.Id = 7;

        var result = await _controller.Edit(vm);

        Assert.IsType<RedirectToActionResult>(result);
        _service.Verify(s => s.UpdateAsync(It.Is<GameSaveDto>(d => d.Id == 7)), Times.Once);
    }

    [Fact]
    public async Task Delete_failure_is_reported_in_TempData()
    {
        _service.Setup(s => s.DeleteAsync(3)).ReturnsAsync(Result.Fail("This game has bets and cannot be deleted."));

        await _controller.Delete(3);

        Assert.Contains("bets", (string)_controller.TempData["Error"]!);
    }
}
