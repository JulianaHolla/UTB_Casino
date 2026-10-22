namespace UTB.Casino.Application.GameLogic;

public record SlotSpin(string[] Reels, decimal Multiplier, string Description);

public static class SlotMachine
{
    // Symbol, weight (sum 100), multiplier for three of a kind (about 92% return to player).
    private static readonly (string Symbol, int Weight, decimal Triple)[] Symbols =
    {
        ("🍒", 30, 5m), ("🍋", 25, 8m), ("🔔", 20, 12m), ("⭐", 15, 25m), ("7️⃣", 10, 100m)
    };

    public static SlotSpin Spin()
    {
        var reels = new[] { Pick(), Pick(), Pick() };
        if (reels[0] == reels[1] && reels[1] == reels[2])
            return new SlotSpin(reels, Symbols.First(s => s.Symbol == reels[0]).Triple, $"Three {reels[0]}");
        if (reels.Count(r => r == "🍒") == 2)
            return new SlotSpin(reels, 2m, "Two 🍒");
        return new SlotSpin(reels, 0m, "No match");
    }

    private static string Pick()
    {
        var roll = RandomProvider.Next(Symbols.Sum(s => s.Weight));
        var cumulative = 0;
        foreach (var s in Symbols)
        {
            cumulative += s.Weight;
            if (roll < cumulative) return s.Symbol;
        }
        return Symbols[0].Symbol;
    }
}
