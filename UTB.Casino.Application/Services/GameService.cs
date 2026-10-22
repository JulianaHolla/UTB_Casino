using Microsoft.Extensions.Logging;
using UTB.Casino.Application.Abstraction;
using UTB.Casino.Application.Dto;
using UTB.Casino.Domain.Constants;
using UTB.Casino.Domain.Entities;

namespace UTB.Casino.Application.Services;

public class GameService : IGameService
{
    private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
    private const long MaxImageBytes = 2 * 1024 * 1024;

    private readonly IGameRepository _games;
    private readonly IUnitOfWork _uow;
    private readonly IFileStorage _files;
    private readonly ILogger<GameService> _logger;

    public GameService(IGameRepository games, IUnitOfWork uow, IFileStorage files, ILogger<GameService> logger)
    { _games = games; _uow = uow; _files = files; _logger = logger; }

    public Task<IList<Game>> GetAllAsync() => _games.GetAllAsync();
    public async Task<IList<Game>> GetActiveAsync() => (await _games.GetAllAsync()).Where(g => g.IsActive).ToList();
    public Task<Game?> GetByIdAsync(int id) => _games.GetByIdAsync(id);
    public Task<Game?> GetBySlugAsync(string slug) => _games.GetBySlugAsync(slug);

    public async Task<Result> CreateAsync(GameSaveDto dto)
    {
        var error = Validate(dto);
        if (error != null) return Result.Fail(error);
        if (!GameSlugs.All.Contains(dto.Slug)) return Result.Fail("Unknown game type.");
        if (await _games.GetBySlugAsync(dto.Slug) != null) return Result.Fail("A game of this type already exists.");

        var (path, imageError) = await StoreImageAsync(dto);
        if (imageError != null) return Result.Fail(imageError);

        await _games.AddAsync(new Game
        {
            Name = dto.Name.Trim(), Slug = dto.Slug, Description = dto.Description?.Trim() ?? "",
            MinBet = dto.MinBet, MaxBet = dto.MaxBet, IsActive = dto.IsActive, ImagePath = path
        });
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Game {Slug} created", dto.Slug);
        return Result.Ok();
    }

    public async Task<Result> UpdateAsync(GameSaveDto dto)
    {
        var game = await _games.GetByIdAsync(dto.Id);
        if (game == null) return Result.Fail("Game not found.");
        var error = Validate(dto);
        if (error != null) return Result.Fail(error);

        var (path, imageError) = await StoreImageAsync(dto);
        if (imageError != null) return Result.Fail(imageError);
        if (path != null) { _files.Delete(game.ImagePath); game.ImagePath = path; }

        game.Name = dto.Name.Trim();
        game.Description = dto.Description?.Trim() ?? "";
        game.MinBet = dto.MinBet;
        game.MaxBet = dto.MaxBet;
        game.IsActive = dto.IsActive;
        _games.Update(game);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Game {GameId} updated", game.Id);
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var game = await _games.GetByIdAsync(id);
        if (game == null) return Result.Fail("Game not found.");
        if (await _games.HasBetsAsync(id)) return Result.Fail("This game has bets and cannot be deleted. Close it instead.");

        _files.Delete(game.ImagePath);
        _games.Remove(game);
        await _uow.SaveChangesAsync();
        _logger.LogInformation("Game {GameId} deleted", id);
        return Result.Ok();
    }

    private static string? Validate(GameSaveDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) return "Name is required.";
        if (dto.MinBet <= 0 || dto.MaxBet < dto.MinBet) return "Min bet must be above 0 and not higher than max bet.";
        return null;
    }

    private async Task<(string? Path, string? Error)> StoreImageAsync(GameSaveDto dto)
    {
        if (dto.ImageStream == null) return (null, null);
        var ext = Path.GetExtension(dto.ImageFileName ?? "").ToLowerInvariant();
        if (!ImageExtensions.Contains(ext)) return (null, "Image must be .jpg, .png, .webp or .gif.");
        if (dto.ImageStream.Length > MaxImageBytes) return (null, "Image must be smaller than 2 MB.");
        return (await _files.SaveImageAsync(dto.ImageStream, ext), null);
    }
}
