using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UTB.Casino.Domain.Entities;

namespace UTB.Casino.Infrastructure.Services;

/// <summary>Blocked players cannot sign in.</summary>
public class CasinoSignInManager : SignInManager<ApplicationUser>
{
    public CasinoSignInManager(UserManager<ApplicationUser> userManager, IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<ApplicationUser>> logger, IAuthenticationSchemeProvider schemes,
        IUserConfirmation<ApplicationUser> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation) { }

    public override async Task<bool> CanSignInAsync(ApplicationUser user) => !user.IsBlocked && await base.CanSignInAsync(user);
}
