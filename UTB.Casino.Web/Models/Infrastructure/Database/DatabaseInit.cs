using UTB.Casino.Web.Models.Entities;

namespace UTB.Casino.Web.Models.Infrastructure.Database;

public static class DatabaseInit
{
    public static void Seed()
    {
        if (DatabaseFake.Games.Any()) return;

        DatabaseFake.Games.Add(new Game { Id = 1, Name = "Lucky Reels", Slug = "Slots", MinBet = 1, MaxBet = 100,
            Description = "Three reels, one line. Triple sevens pay 100 times your bet." });
        DatabaseFake.Games.Add(new Game { Id = 2, Name = "European Roulette", Slug = "Roulette", MinBet = 1, MaxBet = 500,
            Description = "Numbers 0 to 36. Back a single number, a colour, odd or even, or a half." });
        DatabaseFake.Games.Add(new Game { Id = 3, Name = "Blackjack", Slug = "Blackjack", MinBet = 5, MaxBet = 250,
            Description = "Beat the dealer to 21. Blackjack pays 3 to 2, dealer stands on 17." });

        var players = new[]
        {
            new Player { Id = 1, Username = "demo" },
            new Player { Id = 2, Username = "alice" },
            new Player { Id = 3, Username = "bob" },
            new Player { Id = 4, Username = "mallory", IsBlocked = true }
        };
        var starting = new[] { 1000m, 500m, 250m, 50m };

        for (var i = 0; i < players.Length; i++)
        {
            DatabaseFake.Players.Add(players[i]);
            DatabaseFake.Apply(players[i], starting[i], "Deposit", "Welcome credits");
        }
    }
}
