using Microsoft.Extensions.Logging;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Application.Dto;
using UTB.Casino.Domain.Entities;
using UTB.Casino.Domain.Enums;

namespace UTB.Casino.Application.Services;

public class WalletService : IWalletService
{
    private const decimal MaxDeposit = 1000m;
    private readonly IUserRepository _users;
    private readonly IWalletTransactionRepository _tx;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<WalletService> _logger;

    public WalletService(IUserRepository users, IWalletTransactionRepository tx, IUnitOfWork uow, ILogger<WalletService> logger)
    { _users = users; _tx = tx; _uow = uow; _logger = logger; }

    public async Task<decimal> GetBalanceAsync(string userId) => (await _users.GetByIdAsync(userId))?.Balance ?? 0m;

    public async Task<Result> DepositAsync(string userId, decimal amount)
    {
        amount = Math.Round(amount, 2);
        var user = await _users.GetByIdAsync(userId);
        if (user == null) return Result.Fail("Player not found.");
        if (user.IsBlocked) return Result.Fail("Your account is blocked.");
        if (amount < 1 || amount > MaxDeposit) return Result.Fail($"Add between 1 and {MaxDeposit:N0} credits at a time.");

        await ApplyAsync(user, amount, TransactionType.Deposit, "Virtual credits");
        _logger.LogInformation("User {UserId} deposited {Amount}", userId, amount);
        return Result.Ok();
    }

    public async Task<Result> AdjustAsync(string userId, decimal amount, string? note)
    {
        amount = Math.Round(amount, 2);
        var user = await _users.GetByIdAsync(userId);
        if (user == null) return Result.Fail("Player not found.");
        if (amount == 0) return Result.Fail("Amount must not be 0.");
        if (user.Balance + amount < 0) return Result.Fail("Balance cannot go below 0.");

        await ApplyAsync(user, amount, TransactionType.Adjustment, string.IsNullOrWhiteSpace(note) ? "Admin adjustment" : note.Trim());
        _logger.LogInformation("Balance of {UserId} adjusted by {Amount}", userId, amount);
        return Result.Ok();
    }

    public Task<IList<WalletTransaction>> GetUserTransactionsAsync(string userId, int take) => _tx.GetByUserAsync(userId, take);
    public Task<IList<WalletTransaction>> GetAllTransactionsAsync(int take) => _tx.GetRecentAsync(take);
    public Task<WalletTransaction?> GetTransactionAsync(int id) => _tx.GetByIdAsync(id);

    public async Task<Result> UpdateTransactionNoteAsync(int id, string note)
    {
        var tx = await _tx.GetByIdAsync(id);
        if (tx == null) return Result.Fail("Transaction not found.");
        if (string.IsNullOrWhiteSpace(note)) return Result.Fail("Note is required.");
        tx.Note = note.Trim();
        _tx.Update(tx);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    private async Task ApplyAsync(ApplicationUser user, decimal amount, TransactionType type, string note)
    {
        user.Balance += amount;
        await _tx.AddAsync(new WalletTransaction { UserId = user.Id, Amount = amount, Type = type, Note = note });
        await _uow.SaveChangesAsync();
    }
}
