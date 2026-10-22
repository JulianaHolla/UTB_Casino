using Microsoft.Extensions.DependencyInjection;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Application.Services;

namespace UTB.Casino.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IWalletService, WalletService>();
        services.AddScoped<IBetService, BetService>();
        services.AddScoped<ISlotsService, SlotsService>();
        services.AddScoped<IRouletteService, RouletteService>();
        services.AddScoped<IBlackjackService, BlackjackService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddSingleton<IBlackjackRoundStore, InMemoryBlackjackRoundStore>();
        return services;
    }
}
