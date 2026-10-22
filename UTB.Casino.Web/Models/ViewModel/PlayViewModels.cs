using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using UTB.Casino.Application.Dto;
using UTB.Casino.Domain.Entities;

namespace UTB.Casino.Web.Models.ViewModel;

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

public class IndexViewModel
{
    public IList<Game> Games { get; set; } = new List<Game>();
    public IList<Bet> RecentBets { get; set; } = new List<Bet>();
}

/// <summary>Posted by every bet form. BetType/Number are only used by roulette.</summary>
public class BetFormViewModel
{
    [Range(typeof(decimal), "0.01", "100000", ErrorMessage = "Enter a valid bet amount.")]
    public decimal Amount { get; set; }

    public string BetType { get; set; } = "red";

    [Range(0, 36, ErrorMessage = "Pick a number from 0 to 36.")]
    public int? Number { get; set; }
}

public abstract class GameViewModel
{
    public Game Game { get; set; } = new();
    public decimal Amount { get; set; }
    public string? Message { get; set; }
    public bool IsError { get; set; }
}

public class SlotsViewModel : GameViewModel
{
    public string[] Reels { get; set; } = { "🍒", "🍋", "🔔" };
    public bool HasResult { get; set; }
    public decimal Payout { get; set; }
}

public class RouletteViewModel : GameViewModel
{
    public static readonly IList<SelectListItem> BetTypes = new List<SelectListItem>
    {
        new("Red (2x)", "red"), new("Black (2x)", "black"), new("Odd (2x)", "odd"), new("Even (2x)", "even"),
        new("1 to 18 (2x)", "low"), new("19 to 36 (2x)", "high"), new("Single number (36x)", "number")
    };

    public string BetType { get; set; } = "red";
    public int? Number { get; set; }
    public RouletteResultDto? Result { get; set; }
}

public class BlackjackViewModel : GameViewModel
{
    public BlackjackStateDto? State { get; set; }
    public bool InProgress => State is { IsFinished: false };
}

public class WalletViewModel
{
    public decimal Balance { get; set; }
    public IList<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
}

public class DepositViewModel
{
    [Range(typeof(decimal), "1", "1000", ErrorMessage = "Add between 1 and 1000 credits.")]
    public decimal Amount { get; set; } = 100;
}
