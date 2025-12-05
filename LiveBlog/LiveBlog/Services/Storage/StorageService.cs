using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace LiveBlog.Services.Storage;

/// <summary>
/// Сервіс збереження файлів у файловій системі застосунку.
/// Підтримує завантаження та видалення файлів у межах кореня сховища,
/// який обчислюється як <c>Path.Combine(WebRootPath, "storage")</c>.
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// Завантажує файл у вказану «корзину» та повертає відносний шлях
    /// у форматі <c>{bucket}/{yyyy}/{MM}/{dd}/{GUID}_{originalFileName}</c>.
    /// </summary>
    /// <param name="bucket">Назва корзини (логічний розділ сховища), наприклад <c>posts</c>.</param>
    /// <param name="file">Файл для збереження.</param>
    /// <param name="cancellationToken">Токен скасування.</param>
    /// <returns>
    /// Відносний шлях без домену/хоста та без фізичного кореня сховища
    /// (наприклад: <c>posts/2025/12/05/ae1f...c9_my-photo.jpg</c>).
    /// </returns>
    /// <remarks>
    /// Для структури каталогів використовується час у UTC, щоб уникнути залежності від локалі.
    /// Ім'я файлу очищається за допомогою <see cref="Path.GetFileName(string)"/> для запобігання path traversal.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Кинуто, якщо файл порожній або назва корзини некоректна/порожня.
    /// </exception>
    Task<string> UploadAsync(string bucket, IFormFile file, CancellationToken cancellationToken = default);

    /// <summary>
    /// Видаляє файл за відносним шляхом всередині заданої корзини.
    /// </summary>
    /// <param name="bucket">Назва корзини (наприклад: <c>posts</c>).
    /// </param>
    /// <param name="relativePath">Відносний шлях відносно корзини
    /// (наприклад: <c>2025/12/05/GUID_photo.jpg</c>).</param>
    /// <param name="cancellationToken">Токен скасування.</param>
    /// <returns>
    /// <c>true</c>, якщо файл існував і був успішно видалений; інакше <c>false</c>.
    /// </returns>
    Task<bool> DeleteAsync(string bucket, string relativePath, CancellationToken cancellationToken = default);
}

/// <summary>
/// Реалізація <see cref="IStorageService"/> на основі локальної файлової системи.
/// Корінь сховища: <c>Path.Combine(WebRootPath, "storage")</c>.
/// </summary>
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
            throw new ArgumentException("Файл порожній", nameof(file));

        bucket = NormalizeSegment(bucket);
        if (string.IsNullOrWhiteSpace(bucket))
            throw new ArgumentException("Потрібна назва корзини", nameof(bucket));

        var now = DateTime.UtcNow; // зберігання за UTC, шлях не залежить від локалі
        var yyyy = now.ToString("yyyy", CultureInfo.InvariantCulture);
        var mm = now.ToString("MM", CultureInfo.InvariantCulture);
        var dd = now.ToString("dd", CultureInfo.InvariantCulture);

        var originalName = Path.GetFileName(file.FileName); // захист від path traversal у назві
        var uniquePrefix = Guid.NewGuid().ToString("N");
        var targetFileName = $"{uniquePrefix}_{originalName}";

        // Абсолютний шлях збереження
        var physicalDir = Path.Combine(StorageRoot, bucket, yyyy, mm, dd);
        Directory.CreateDirectory(physicalDir);

        var physicalPath = Path.Combine(physicalDir, targetFileName);
        await using (var stream = new FileStream(physicalPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        // Відносний шлях відносно кореня сховища (без StorageRoot)
        var relativePath = string.Join('/', new[] { bucket, yyyy, mm, dd, targetFileName });
        return relativePath;
    }

    public Task<bool> DeleteAsync(string bucket, string relativePath, CancellationToken cancellationToken = default)
    {
        bucket = NormalizeSegment(bucket);
        if (string.IsNullOrWhiteSpace(bucket))
            throw new ArgumentException("Потрібна назва корзини", nameof(bucket));

        // Очікується шлях відносно корзини (наприклад: 2025/12/05/GUID_photo.jpg)
        var safeRelative = NormalizeRelativePath(relativePath);
        if (string.IsNullOrWhiteSpace(safeRelative))
            return Task.FromResult(false);

        var physicalPath = Path.Combine(StorageRoot, bucket, safeRelative);

        // Примусово переконуємось, що шлях залишається всередині StorageRoot
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
        // Замінюємо зворотні слеші, прибираємо початкові/кінцеві слеші/пробіли
        segment = segment.Replace('\\', '/').Trim().Trim('/');
        // Забороняємо піднімання по каталогах
        if (segment.Contains(".."))
            throw new ArgumentException("Некоректний сегмент шляху");
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