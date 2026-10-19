using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Web.Models.Infrastructure.Database;

namespace UTB.Casino.Web.Areas.Admin.Controllers;

[Area("Admin")]
public class TransactionController : Controller
{
    public IActionResult Select()
    {
        ViewBag.Players = DatabaseFake.Players.ToDictionary(p => p.Id, p => p.Username);
        return View(DatabaseFake.Transactions.OrderByDescending(t => t.CreatedAt).Take(100).ToList());
    }
}
