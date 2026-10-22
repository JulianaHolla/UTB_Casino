using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Application.Abstraction;

namespace UTB.Casino.Web.Components;

public class BalanceViewComponent : ViewComponent
{
    private readonly IWalletService _wallet;
    public BalanceViewComponent(IWalletService wallet) => _wallet = wallet;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var id = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id == null) return Content(string.Empty);
        return View(await _wallet.GetBalanceAsync(id));
    }
}
