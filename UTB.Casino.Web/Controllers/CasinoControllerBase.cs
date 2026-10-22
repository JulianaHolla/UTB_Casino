using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UTB.Casino.Web.Controllers;

[Authorize]
public abstract class CasinoControllerBase : Controller
{
    protected string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
}
