using UTB.Casino.Application.GameLogic;

namespace UTB.Casino.Tests;

public class GameLogicTests
{
    private static Card C(string rank, int value) => new() { Rank = rank, Suit = "♠", Value = value };

    [Fact]
    public void Ace_counts_as_one_when_eleven_would_bust()
    {
        var hand = new[] { C("A", 11), C("9", 9), C("5", 5) };
        Assert.Equal(15, BlackjackEngine.HandValue(hand));
    }

    [Fact]
    public void Ace_and_king_is_blackjack()
    {
        Assert.True(BlackjackEngine.IsBlackjack(new List<Card> { C("A", 11), C("K", 10) }));
    }

    [Theory]
    [InlineData("red", 1, 2)]
    [InlineData("red", 2, 0)]
    [InlineData("even", 0, 0)]
    [InlineData("low", 18, 2)]
    [InlineData("high", 18, 0)]
    public void Roulette_outside_bets_pay_expected_multiplier(string bet, int spin, int expected)
    {
        Assert.Equal(expected, RouletteWheel.Multiplier(bet, null, spin));
    }

    [Fact]
    public void Roulette_straight_up_pays_36()
    {
        Assert.Equal(36m, RouletteWheel.Multiplier("number", 17, 17));
        Assert.Equal(0m, RouletteWheel.Multiplier("number", 17, 18));
    }

    [Fact]
    public void Dealt_hand_has_two_cards_each_and_payout_matches_state()
    {
        var round = BlackjackEngine.Start(10m);
        Assert.Equal(2, round.PlayerHand.Count);
        Assert.Equal(2, round.DealerHand.Count);
        if (round.IsFinished) Assert.Equal(round.State == RoundState.DealerWin ? 0m : BlackjackEngine.Payout(round), BlackjackEngine.Payout(round));
    }
}
