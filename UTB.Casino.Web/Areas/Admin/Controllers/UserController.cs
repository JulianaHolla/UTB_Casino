using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Application.Dto;
using UTB.Casino.Domain.Constants;
using UTB.Casino.Web.Areas.Admin.Models;

namespace UTB.Casino.Web.Areas.Admin.Controllers;

/// <summary>Users, roles and balances are critical data: Admin only (Manager has no access).</summary>
[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public class UserController : Controller
{
    private readonly IAccountService _accounts;
    private readonly IWalletService _wallet;
    private readonly ILogger<UserController> _logger;

    public UserController(IAccountService accounts, IWalletService wallet, ILogger<UserController> logger)
    { _accounts = accounts; _wallet = wallet; _logger = logger; }

    public async Task<IActionResult> Select() => View(await _accounts.GetUsersAsync());

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _accounts.GetUserAsync(id);
        if (user == null) return NotFound();
        return View(WithRoles(new UserEditViewModel
        {
            Id = user.Id, UserName = user.UserName, Email = user.Email, DisplayName = user.DisplayName,
            Role = user.Role, IsBlocked = user.IsBlocked, Balance = user.Balance
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UserEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(WithRoles(vm));

        var result = await _accounts.UpdateUserAsync(new UserEditDto
        {
            Id = vm.Id, UserName = vm.UserName, Email = vm.Email, DisplayName = vm.DisplayName,
            IsBlocked = vm.IsBlocked, Role = vm.Role
        });
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError("", error);
            return View(WithRoles(vm));
        }
        _logger.LogInformation("User {UserId} edited by {Admin}", vm.Id, User.Identity?.Name);
        return RedirectToAction(nameof(Select));
    }

    [HttpPost]
    public async Task<IActionResult> Adjust(AdjustBalanceViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Enter a valid amount.";
            return RedirectToAction(nameof(Edit), new { id = vm.UserId });
        }
        var result = await _wallet.AdjustAsync(vm.UserId, vm.Amount, vm.Note);
        TempData[result.Succeeded ? "Success" : "Error"] = result.Succeeded ? "Balance adjusted." : result.Error;
        return RedirectToAction(nameof(Edit), new { id = vm.UserId });
    }

    private static UserEditViewModel WithRoles(UserEditViewModel vm)
    {
        vm.RoleItems = Roles.All.Select(r => new SelectListItem(r, r)).ToList();
        return vm;
    }
}
