using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Application.Dto;
using UTB.Casino.Domain.Entities;
using UTB.Casino.Web.Models.ViewModel;

namespace UTB.Casino.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accounts;
    private readonly SignInManager<ApplicationUser> _signIn;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAccountService accounts, SignInManager<ApplicationUser> signIn, ILogger<AccountController> logger)
    { _accounts = accounts; _signIn = signIn; _logger = logger; }

    [HttpGet, AllowAnonymous]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _accounts.RegisterAsync(new RegisterDto
        {
            UserName = vm.UserName, Email = vm.Email, DisplayName = vm.DisplayName,
            DateOfBirth = vm.DateOfBirth!.Value, Password = vm.Password
        });
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError("", error);
            return View(vm);
        }

        await _signIn.PasswordSignInAsync(vm.UserName, vm.Password, isPersistent: false, lockoutOnFailure: false);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet, AllowAnonymous]
    public IActionResult Login(string? returnUrl = null) => View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await _signIn.PasswordSignInAsync(vm.UserName, vm.Password, vm.RememberMe, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            _logger.LogInformation("User {UserName} signed in", vm.UserName);
            return !string.IsNullOrEmpty(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl)
                ? LocalRedirect(vm.ReturnUrl) : RedirectToAction("Index", "Home");
        }

        _logger.LogWarning("Failed sign-in for {UserName}", vm.UserName);
        ModelState.AddModelError("", result.IsNotAllowed ? "This account is blocked." : "Invalid username or password.");
        return View(vm);
    }

    [HttpPost, Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AccessDenied() => View();
}
