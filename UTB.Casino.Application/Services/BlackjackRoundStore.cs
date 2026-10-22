using System.Collections.Concurrent;
using UTB.Casino.Application.GameLogic;

namespace UTB.Casino.Application.Services;

public interface IBlackjackRoundStore
{
    BlackjackRound? Get(string userId);
    void Set(string userId, BlackjackRound round);
}

/// <summary>Keeps the running hand per player (singleton, in memory).</summary>
public class InMemoryBlackjackRoundStore : IBlackjackRoundStore
{
    private readonly ConcurrentDictionary<string, BlackjackRound> _rounds = new();
    public BlackjackRound? Get(string userId) => _rounds.GetValueOrDefault(userId);
    public void Set(string userId, BlackjackRound round) => _rounds[userId] = round;
}
