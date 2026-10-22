using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Web.Models.ViewModel;

namespace UTB.Casino.Web.Controllers;

public class HomeController : Controller
{
    private readonly IGameService _games;
    private readonly IBetService _bets;

    public HomeController(IGameService games, IBetService bets) { _games = games; _bets = bets; }

    public async Task<IActionResult> Index()
    {
        var vm = new IndexViewModel { Games = await _games.GetActiveAsync() };
        if (User.Identity?.IsAuthenticated == true)
            vm.RecentBets = await _bets.GetUserBetsAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!, 5);
        return View(vm);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
