using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Domain.Entities;
using UTB.Casino.Infrastructure.Data;
using UTB.Casino.Infrastructure.Repositories;
using UTB.Casino.Infrastructure.Services;

namespace UTB.Casino.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config, string webRootPath)
    {
        services.AddDbContext<CasinoDbContext>(o =>
            o.UseSqlite(config.GetConnectionString("Default") ?? "Data Source=casino.db"));

        services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            {
                o.Password.RequiredLength = 8;
                o.Password.RequireNonAlphanumeric = false;
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<CasinoDbContext>()
            .AddSignInManager<CasinoSignInManager>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(o =>
        {
            o.LoginPath = "/Account/Login";
            o.AccessDeniedPath = "/Account/AccessDenied";
        });

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IBetRepository, BetRepository>();
        services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddUnitOfWork<CasinoDbContext>();

        services.AddSingleton(new FileStorageOptions { RootPath = webRootPath });
        services.AddSingleton<IFileStorage, LocalFileStorage>();
        return services;
    }

    /// <summary>Another DbContext can be given its own unit of work by calling this again.</summary>
    public static IServiceCollection AddUnitOfWork<TContext>(this IServiceCollection services) where TContext : DbContext
    {
        services.AddScoped<IUnitOfWork, UnitOfWork<TContext>>();
        return services;
    }
}
