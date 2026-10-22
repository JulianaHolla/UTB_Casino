using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UTB.Casino.Domain.Constants;
using UTB.Casino.Domain.Entities;
using UTB.Casino.Domain.Enums;

namespace UTB.Casino.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<CasinoDbContext>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        // Applies migrations when they exist; falls back to creating the schema from the model.
        if (ctx.Database.GetMigrations().Any()) await ctx.Database.MigrateAsync();
        else await ctx.Database.EnsureCreatedAsync();

        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var role in Roles.All)
            if (!await roles.RoleExistsAsync(role)) await roles.CreateAsync(new IdentityRole(role));

        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        await EnsureUserAsync(users, ctx, "admin", "Administrator", Roles.Admin, config["Seed:AdminPassword"] ?? "Admin123!", 0);
        await EnsureUserAsync(users, ctx, "manager", "Manager", Roles.Manager, config["Seed:ManagerPassword"] ?? "Manager123!", 0);
        await EnsureUserAsync(users, ctx, "demo", "Demo Player", Roles.Customer, config["Seed:CustomerPassword"] ?? "Customer123!", 1000);

        if (!await ctx.Games.AnyAsync())
        {
            ctx.Games.AddRange(
                new Game { Name = "Lucky Reels", Slug = GameSlugs.Slots, MinBet = 1, MaxBet = 100, Description = "Three reels, one line. Triple sevens pay 100 times your bet." },
                new Game { Name = "European Roulette", Slug = GameSlugs.Roulette, MinBet = 1, MaxBet = 500, Description = "Numbers 0 to 36. Back a number, a colour, odd or even, or a half." },
                new Game { Name = "Blackjack", Slug = GameSlugs.Blackjack, MinBet = 5, MaxBet = 250, Description = "Beat the dealer to 21. Blackjack pays 3 to 2, dealer stands on 17." });
            await ctx.SaveChangesAsync();
        }
    }

    private static async Task EnsureUserAsync(UserManager<ApplicationUser> users, CasinoDbContext ctx, string userName,
        string displayName, string role, string password, decimal startingBalance)
    {
        if (await users.FindByNameAsync(userName) != null) return;
        var user = new ApplicationUser
        {
            UserName = userName, Email = $"{userName}@casino.local", EmailConfirmed = true, DisplayName = displayName,
            DateOfBirth = new DateTime(1990, 1, 1), Balance = startingBalance
        };
        var created = await users.CreateAsync(user, password);
        if (!created.Succeeded) throw new InvalidOperationException(string.Join("; ", created.Errors.Select(e => e.Description)));
        await users.AddToRoleAsync(user, role);

        if (startingBalance > 0)
        {
            ctx.WalletTransactions.Add(new WalletTransaction { UserId = user.Id, Amount = startingBalance, Type = TransactionType.Deposit, Note = "Welcome credits" });
            await ctx.SaveChangesAsync();
        }
    }
}
