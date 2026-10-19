using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Web.Models.Infrastructure.Database;
using UTB.Casino.Web.Models.ViewModel;

namespace UTB.Casino.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        var player = DatabaseFake.CurrentPlayer;
        return View(new IndexViewModel
        {
            Player = player,
            Games = DatabaseFake.Games.Where(g => g.IsActive).ToList(),
            RecentBets = DatabaseFake.Bets.Where(b => b.PlayerId == player.Id)
                                          .OrderByDescending(b => b.PlacedAt).Take(5).ToList()
        });
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
