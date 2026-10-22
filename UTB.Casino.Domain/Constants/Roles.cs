namespace UTB.Casino.Domain.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Customer = "Customer";
    public const string AdminOrManager = Admin + "," + Manager;

    public static readonly string[] All = { Admin, Manager, Customer };
}

/// <summary>Game slug = name of the MVC controller that plays the game.</summary>
public static class GameSlugs
{
    public const string Slots = "Slots";
    public const string Roulette = "Roulette";
    public const string Blackjack = "Blackjack";

    public static readonly string[] All = { Slots, Roulette, Blackjack };
}
