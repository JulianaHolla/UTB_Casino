using UTB.Casino.Domain.Entities;

namespace UTB.Casino.Application.Abstraction;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IList<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}

public interface IGameRepository : IRepository<Game>
{
    Task<Game?> GetBySlugAsync(string slug);
    Task<bool> HasBetsAsync(int gameId);
}

public interface IBetRepository : IRepository<Bet>
{
    Task<IList<Bet>> GetByUserAsync(string userId, int take);
    Task<IList<Bet>> GetRecentAsync(int take);
}

public interface IWalletTransactionRepository : IRepository<WalletTransaction>
{
    Task<IList<WalletTransaction>> GetByUserAsync(string userId, int take);
    Task<IList<WalletTransaction>> GetRecentAsync(int take);
}

public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task<IList<ApplicationUser>> GetAllAsync();
}

//Commits all repository changes made through one DbContext as one unit
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
    Task ExecuteInTransactionAsync(Func<Task> action);
}

public interface IFileStorage
{
    //Stores an image and returns its public relative path
    Task<string> SaveImageAsync(Stream content, string extension);
    void Delete(string? relativePath);
}
