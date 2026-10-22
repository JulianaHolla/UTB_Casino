using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Application.Dto;
using UTB.Casino.Domain.Constants;
using UTB.Casino.Domain.Entities;

namespace UTB.Casino.Application.Services;

public class AccountService : IAccountService
{
    private const int MinimumAge = 18;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserRepository _users;
    private readonly ILogger<AccountService> _logger;

    public AccountService(UserManager<ApplicationUser> userManager, IUserRepository users, ILogger<AccountService> logger)
    { _userManager = userManager; _users = users; _logger = logger; }

    public async Task<Result> RegisterAsync(RegisterDto dto)
    {
        var today = DateTime.Today;
        var age = today.Year - dto.DateOfBirth.Year;
        if (dto.DateOfBirth.Date > today.AddYears(-age)) age--;
        if (age < MinimumAge) return Result.Fail($"You must be at least {MinimumAge} years old.");

        var user = new ApplicationUser
        {
            UserName = dto.UserName, Email = dto.Email, DisplayName = dto.DisplayName,
            DateOfBirth = dto.DateOfBirth, CreatedAt = DateTime.UtcNow
        };
        var created = await _userManager.CreateAsync(user, dto.Password);
        if (!created.Succeeded) return Result.Fail(created.Errors.Select(e => e.Description));

        await _userManager.AddToRoleAsync(user, Roles.Customer);
        _logger.LogInformation("User {UserName} registered", dto.UserName);
        return Result.Ok();
    }

    public async Task<IList<UserDto>> GetUsersAsync()
    {
        var list = new List<UserDto>();
        foreach (var u in await _users.GetAllAsync()) list.Add(await ToDtoAsync(u));
        return list;
    }

    public async Task<UserDto?> GetUserAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        return user == null ? null : await ToDtoAsync(user);
    }

    public async Task<Result> UpdateUserAsync(UserEditDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.Id);
        if (user == null) return Result.Fail("User not found.");
        if (!Roles.All.Contains(dto.Role)) return Result.Fail("Unknown role.");

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Contains(Roles.Admin) && dto.Role != Roles.Admin
            && (await _userManager.GetUsersInRoleAsync(Roles.Admin)).Count <= 1)
            return Result.Fail("The last administrator cannot lose the Admin role.");

        // Only profile fields are touched - the password hash is never read or written here.
        var errors = new List<string>();
        if (user.UserName != dto.UserName) errors.AddRange((await _userManager.SetUserNameAsync(user, dto.UserName)).Errors.Select(e => e.Description));
        if (user.Email != dto.Email) errors.AddRange((await _userManager.SetEmailAsync(user, dto.Email)).Errors.Select(e => e.Description));
        if (errors.Count > 0) return Result.Fail(errors);

        user.DisplayName = dto.DisplayName;
        user.IsBlocked = dto.IsBlocked;
        var updated = await _userManager.UpdateAsync(user);
        if (!updated.Succeeded) return Result.Fail(updated.Errors.Select(e => e.Description));

        if (!currentRoles.Contains(dto.Role) || currentRoles.Count != 1)
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, dto.Role);
        }
        _logger.LogInformation("User {UserId} updated by admin", dto.Id);
        return Result.Ok();
    }

    private async Task<UserDto> ToDtoAsync(ApplicationUser u) => new()
    {
        Id = u.Id, UserName = u.UserName ?? "", Email = u.Email ?? "", DisplayName = u.DisplayName,
        Balance = u.Balance, IsBlocked = u.IsBlocked, CreatedAt = u.CreatedAt,
        Role = (await _userManager.GetRolesAsync(u)).FirstOrDefault() ?? Roles.Customer
    };
}
