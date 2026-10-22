using UTB.Casino.Application.Abstraction;

namespace UTB.Casino.Infrastructure.Services;

public class FileStorageOptions
{
    public string RootPath { get; set; } = "";
}

public class LocalFileStorage : IFileStorage
{
    private const string Folder = "uploads";
    private readonly string _root;

    public LocalFileStorage(FileStorageOptions options) => _root = Path.Combine(options.RootPath, Folder);

    public async Task<string> SaveImageAsync(Stream content, string extension)
    {
        Directory.CreateDirectory(_root);
        var name = $"{Guid.NewGuid():N}{extension}";
        await using var file = File.Create(Path.Combine(_root, name));
        await content.CopyToAsync(file);
        return $"/{Folder}/{name}";
    }

    public void Delete(string? relativePath)
    {
        if (string.IsNullOrEmpty(relativePath) || !relativePath.StartsWith($"/{Folder}/")) return;
        var path = Path.Combine(_root, Path.GetFileName(relativePath));
        if (File.Exists(path)) File.Delete(path);
    }
}
