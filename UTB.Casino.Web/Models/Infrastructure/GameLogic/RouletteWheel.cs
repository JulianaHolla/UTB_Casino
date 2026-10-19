namespace UTB.Casino.Web.Models.Infrastructure.GameLogic;

public static class RouletteWheel
{
    private static readonly HashSet<int> Reds = new() { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };

    public static readonly string[] BetTypes = { "number", "red", "black", "odd", "even", "low", "high" };

    public static int Spin() => RandomProvider.Next(37);

    public static string ColorOf(int number) => number == 0 ? "green" : Reds.Contains(number) ? "red" : "black";

    public static bool IsValidBet(string betType, int? number) =>
        BetTypes.Contains(betType) && (betType != "number" || number is >= 0 and <= 36);

    /// <summary>Total return per 1 staked (stake included). 0 = lost.</summary>
    public static decimal Multiplier(string betType, int? number, int spin) => betType switch
    {
        "number" => number == spin ? 36m : 0m,
        "red" => ColorOf(spin) == "red" ? 2m : 0m,
        "black" => ColorOf(spin) == "black" ? 2m : 0m,
        "odd" => spin % 2 == 1 ? 2m : 0m,
        "even" => spin != 0 && spin % 2 == 0 ? 2m : 0m,
        "low" => spin is >= 1 and <= 18 ? 2m : 0m,
        "high" => spin is >= 19 and <= 36 ? 2m : 0m,
        _ => 0m
    };
}
