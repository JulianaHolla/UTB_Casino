namespace UTB.Casino.Application.Dto;

public class Result
{
    public bool Succeeded { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
    public string Error => string.Join(" ", Errors);

    public static Result Ok() => new() { Succeeded = true };
    public static Result Fail(params string[] errors) => new() { Errors = errors };
    public static Result Fail(IEnumerable<string> errors) => new() { Errors = errors.ToArray() };
}

public class Result<T> : Result
{
    public T? Value { get; init; }
    public static Result<T> Ok(T value) => new() { Succeeded = true, Value = value };
    public new static Result<T> Fail(params string[] errors) => new() { Errors = errors };
}

public record SlotsResultDto(string[] Reels, decimal Payout, string Description);
public record RouletteResultDto(int Number, string Color, decimal Payout);

public record CardDto(string Rank, string Suit, bool IsRed, bool IsHidden);
public record BlackjackStateDto(IList<CardDto> PlayerCards, IList<CardDto> DealerCards, int PlayerValue,
                                int? DealerValue, bool IsFinished, string Outcome, decimal Bet, decimal Payout);

public class RegisterDto
{
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public DateTime DateOfBirth { get; set; }
    public string Password { get; set; } = "";
}

public class UserDto
{
    public string Id { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public decimal Balance { get; set; }
    public bool IsBlocked { get; set; }
    public string Role { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

/// <summary>Admin edit of a user. There is deliberately no password / hash field.</summary>
public class UserEditDto
{
    public string Id { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public bool IsBlocked { get; set; }
    public string Role { get; set; } = "";
}

public class GameSaveDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal MinBet { get; set; }
    public decimal MaxBet { get; set; }
    public bool IsActive { get; set; }
    public Stream? ImageStream { get; set; }
    public string? ImageFileName { get; set; }
}
