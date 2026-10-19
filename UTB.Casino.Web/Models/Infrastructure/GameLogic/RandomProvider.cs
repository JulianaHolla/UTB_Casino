using System.Security.Cryptography;

namespace UTB.Casino.Web.Models.Infrastructure.GameLogic;

/// <summary>Shared RNG for every game (cryptographically strong, unbiased).</summary>
public static class RandomProvider
{
    /// <summary>Returns 0 .. maxExclusive-1.</summary>
    public static int Next(int maxExclusive) => RandomNumberGenerator.GetInt32(maxExclusive);
}
