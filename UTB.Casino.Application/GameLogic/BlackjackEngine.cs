namespace UTB.Casino.Application.GameLogic;

public class Card
{
    public string Suit { get; init; } = "";
    public string Rank { get; init; } = "";
    public int Value { get; init; }
    public bool IsRed => Suit is "♥" or "♦";
}

public enum RoundState { InProgress, PlayerBlackjack, PlayerWin, DealerWin, Push }

public class BlackjackRound
{
    public decimal Bet { get; set; }
    public List<Card> Deck { get; set; } = new();
    public List<Card> PlayerHand { get; } = new();
    public List<Card> DealerHand { get; } = new();
    public RoundState State { get; set; } = RoundState.InProgress;
    public bool IsFinished => State != RoundState.InProgress;
}

public static class BlackjackEngine
{
    private static readonly string[] Suits = { "♠", "♥", "♦", "♣" };
    private static readonly string[] Ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    public static BlackjackRound Start(decimal bet)
    {
        var round = new BlackjackRound { Bet = bet, Deck = NewShuffledDeck() };
        round.PlayerHand.Add(Draw(round));
        round.DealerHand.Add(Draw(round));
        round.PlayerHand.Add(Draw(round));
        round.DealerHand.Add(Draw(round));

        var player = IsBlackjack(round.PlayerHand);
        var dealer = IsBlackjack(round.DealerHand);
        if (player && dealer) round.State = RoundState.Push;
        else if (player) round.State = RoundState.PlayerBlackjack;
        else if (dealer) round.State = RoundState.DealerWin;
        return round;
    }

    public static void Hit(BlackjackRound round)
    {
        if (round.IsFinished) return;
        round.PlayerHand.Add(Draw(round));
        var value = HandValue(round.PlayerHand);
        if (value > 21) round.State = RoundState.DealerWin;
        else if (value == 21) Stand(round);
    }

    public static void Stand(BlackjackRound round)
    {
        if (round.IsFinished) return;
        while (HandValue(round.DealerHand) < 17) round.DealerHand.Add(Draw(round));
        var player = HandValue(round.PlayerHand);
        var dealer = HandValue(round.DealerHand);
        round.State = dealer > 21 || player > dealer ? RoundState.PlayerWin
                    : player == dealer ? RoundState.Push
                    : RoundState.DealerWin;
    }

    /// <summary>Total returned to the player, stake included.</summary>
    public static decimal Payout(BlackjackRound round) => round.State switch
    {
        RoundState.PlayerBlackjack => round.Bet * 2.5m,
        RoundState.PlayerWin => round.Bet * 2m,
        RoundState.Push => round.Bet,
        _ => 0m
    };

    public static int HandValue(IEnumerable<Card> hand)
    {
        var cards = hand.ToList();
        var total = cards.Sum(c => c.Value);
        var aces = cards.Count(c => c.Rank == "A");
        while (total > 21 && aces-- > 0) total -= 10;
        return total;
    }

    public static bool IsBlackjack(IList<Card> hand) => hand.Count == 2 && HandValue(hand) == 21;

    private static Card Draw(BlackjackRound round)
    {
        var card = round.Deck[^1];
        round.Deck.RemoveAt(round.Deck.Count - 1);
        return card;
    }

    private static List<Card> NewShuffledDeck()
    {
        var deck = (from s in Suits
                    from r in Ranks
                    select new Card { Suit = s, Rank = r, Value = r == "A" ? 11 : r is "J" or "Q" or "K" ? 10 : int.Parse(r) })
                   .ToList();
        for (var i = deck.Count - 1; i > 0; i--)
        {
            var j = RandomProvider.Next(i + 1);
            (deck[i], deck[j]) = (deck[j], deck[i]);
        }
        return deck;
    }
}
