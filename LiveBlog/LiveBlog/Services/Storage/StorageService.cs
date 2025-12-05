using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace LiveBlog.Services.Storage;

public interface IStorageService
{
    // Загружает файл в корзину и возвращает относительный путь вида:
    // {bucket}/{yyyy}/{MM}/{dd}/{GUID}_{originalFileName}
    Task<string> UploadAsync(string bucket, IFormFile file, CancellationToken cancellationToken = default);

    // Удаляет файл, где relativePath — путь относительно корзины (например: 2025/12/05/GUID_photo.jpg)
    Task<bool> DeleteAsync(string bucket, string relativePath, CancellationToken cancellationToken = default);
}

public class StorageService : IStorageService
{
    private readonly IWebHostEnvironment _env;

    public StorageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    private string StorageRoot => Path.Combine(_env.WebRootPath, "storage");

    public async Task<string> UploadAsync(string bucket, IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty", nameof(file));

        bucket = NormalizeSegment(bucket);
        if (string.IsNullOrWhiteSpace(bucket))
            throw new ArgumentException("Bucket is required", nameof(bucket));

        var now = DateTime.UtcNow; // хранение по UTC, ссылка не зависит от локали
        var yyyy = now.ToString("yyyy", CultureInfo.InvariantCulture);
        var mm = now.ToString("MM", CultureInfo.InvariantCulture);
        var dd = now.ToString("dd", CultureInfo.InvariantCulture);

        var originalName = Path.GetFileName(file.FileName); // защита от path traversal в имени
        var uniquePrefix = Guid.NewGuid().ToString("N");
        var targetFileName = $"{uniquePrefix}_{originalName}";

        // Абсолютный путь сохранения
        var physicalDir = Path.Combine(StorageRoot, bucket, yyyy, mm, dd);
        Directory.CreateDirectory(physicalDir);

        var physicalPath = Path.Combine(physicalDir, targetFileName);
        await using (var stream = new FileStream(physicalPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        // Относительный путь относительно корня хранилища (без StorageRoot)
        var relativePath = string.Join('/', new[] { bucket, yyyy, mm, dd, targetFileName });
        return relativePath;
    }

    public Task<bool> DeleteAsync(string bucket, string relativePath, CancellationToken cancellationToken = default)
    {
        bucket = NormalizeSegment(bucket);
        if (string.IsNullOrWhiteSpace(bucket))
            throw new ArgumentException("Bucket is required", nameof(bucket));

        // relativePath ожидается относительно корзины (например: 2025/12/05/GUID_photo.jpg)
        var safeRelative = NormalizeRelativePath(relativePath);
        if (string.IsNullOrWhiteSpace(safeRelative))
            return Task.FromResult(false);

        var physicalPath = Path.Combine(StorageRoot, bucket, safeRelative);

        // Принудительно убеждаемся, что путь остаётся внутри StorageRoot
        var full = Path.GetFullPath(physicalPath);
        var root = Path.GetFullPath(StorageRoot);
        if (!full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            return Task.FromResult(false);

        if (File.Exists(full))
        {
            File.Delete(full);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    private static string NormalizeSegment(string? segment)
    {
        if (string.IsNullOrWhiteSpace(segment)) return string.Empty;
        // Заменяем обратные слеши, убираем ведущие/замыкающие слеши/пробелы
        segment = segment.Replace('\\', '/').Trim().Trim('/');
        // Запрещаем восхождение по каталогам
        if (segment.Contains(".."))
            throw new ArgumentException("Invalid segment");
        return segment;
    }

    private static string NormalizeRelativePath(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return string.Empty;
        var p = relativePath.Replace('\\', '/').Trim().Trim('/');
        if (p.Contains("..")) return string.Empty;
        return p;
    }
}