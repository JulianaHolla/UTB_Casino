using UTB.Casino.Application.Dto;
using UTB.Casino.Domain.Entities;

namespace UTB.Casino.Application.Abstraction;

public interface IGameService
{
    Task<IList<Game>> GetAllAsync();
    Task<IList<Game>> GetActiveAsync();
    Task<Game?> GetByIdAsync(int id);
    Task<Game?> GetBySlugAsync(string slug);
    Task<Result> CreateAsync(GameSaveDto dto);
    Task<Result> UpdateAsync(GameSaveDto dto);
    Task<Result> DeleteAsync(int id);
}

public interface IWalletService
{
    Task<decimal> GetBalanceAsync(string userId);
    Task<Result> DepositAsync(string userId, decimal amount);
    Task<Result> AdjustAsync(string userId, decimal amount, string? note);
    Task<IList<WalletTransaction>> GetUserTransactionsAsync(string userId, int take);
    Task<IList<WalletTransaction>> GetAllTransactionsAsync(int take);
    Task<WalletTransaction?> GetTransactionAsync(int id);
    Task<Result> UpdateTransactionNoteAsync(int id, string note);
}

public interface IBetService
{
    Task<IList<Bet>> GetUserBetsAsync(string userId, int take);
    Task<IList<Bet>> GetAllAsync(int take);
    Task<Bet?> GetByIdAsync(int id);
    Task<Result> UpdateResultAsync(int id, string result);
    Task<Result> DeleteAsync(int id);
}

public interface ISlotsService
{
    Task<Result<SlotsResultDto>> SpinAsync(string userId, decimal amount);
}

public interface IRouletteService
{
    Task<Result<RouletteResultDto>> SpinAsync(string userId, decimal amount, string betType, int? number);
}

public interface IBlackjackService
{
    BlackjackStateDto? GetState(string userId);
    Task<Result> DealAsync(string userId, decimal amount);
    Task<Result> HitAsync(string userId);
    Task<Result> StandAsync(string userId);
}

public interface IAccountService
{
    Task<Result> RegisterAsync(RegisterDto dto);
    Task<IList<UserDto>> GetUsersAsync();
    Task<UserDto?> GetUserAsync(string id);
    Task<Result> UpdateUserAsync(UserEditDto dto);
}
