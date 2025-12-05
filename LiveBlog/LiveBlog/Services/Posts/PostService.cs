using LiveBlog.Models.Posts;
using LiveBlog.Models.Media;
using LiveBlog.Repositories.Posts;
using LiveBlog.Services.Storage;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace LiveBlog.Services.Posts;

/// <summary>
/// Реалізація сервісу доменної логіки для роботи з дописами.
/// Відповідає за валідацію та делегує доступ до даних репозиторію.
/// </summary>
public class PostService : IPostService
{
    private readonly IPostRepository _repo;
    private readonly IStorageService _storage;

    public PostService(IPostRepository repo, IStorageService storage)
    {
        _repo = repo;
        _storage = storage;
    }

    /// <inheritdoc />
    public async Task<SmallPostResponse> CreateAsync(CreatePostRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Slug))
            throw new ArgumentException("Slug обов'язковий", nameof(request.Slug));
        if (string.IsNullOrWhiteSpace(request.Content))
            throw new ArgumentException("Content обов'язковий", nameof(request.Content));

        // Перевірка унікальності slug
        var exists = await _repo.ExistsAsync(p => p.Slug == request.Slug.Trim(), cancellationToken);
        if (exists)
            throw new InvalidOperationException("Допис із таким slug вже існує");

        var entity = PostMapper.FromCreateRequest(request);
        entity.Slug = entity.Slug.Trim();
        entity.Content = entity.Content.Trim();

        // Зберігаємо файли у сховище та підміняємо назви файлів на збережені шляхи
        if (request.Files is { Count: > 0 })
        {
            entity.MediaFiles.Clear();
            foreach (var file in request.Files.Where(f => f is { Length: > 0 }))
            {
                var savedPath = await _storage.UploadAsync("posts", file, cancellationToken);
                entity.MediaFiles.Add(new PostMediaFileEntity
                {
                    FileName = savedPath
                });
            }
        }

        entity = await _repo.AddAsync(entity, cancellationToken);

        // Перечитуємо з медіа, щоб сформувати коректну відповідь
        var withMedia = await _repo.GetWithMediaByIdAsync(entity.Id, cancellationToken) ?? entity;
        return PostMapper.ToSmallResponse(withMedia);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(int id, UpdatePostRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetWithMediaByIdAsync(id, cancellationToken);
        if (entity is null) return false;

        // Якщо змінюємо slug — перевіряємо унікальність
        if (!string.IsNullOrWhiteSpace(request.Slug) && !string.Equals(request.Slug, entity.Slug, StringComparison.Ordinal))
        {
            var slugExists = await _repo.ExistsAsync(p => p.Slug == request.Slug!, cancellationToken);
            if (slugExists)
                throw new InvalidOperationException("Допис із таким slug вже існує");
        }

        // Якщо файли не передані (null) — не змінюємо вкладення
        // Якщо порожній список — очищаємо вкладення
        // Якщо є файли — перезберігаємо у сховище та замінюємо колекцію медіафайлів
        if (request.Files is null)
        {
            // застосовуємо лише текстові зміни
            PostMapper.ApplyUpdate(entity, new UpdatePostRequest
            {
                Slug = request.Slug,
                Content = request.Content,
                Files = null
            });
        }
        else if (request.Files.Count == 0)
        {
            // очищення вкладень
            PostMapper.ApplyUpdate(entity, new UpdatePostRequest
            {
                Slug = request.Slug,
                Content = request.Content,
                Files = request.Files // порожня колекція — призведе до Clear()
            });
        }
        else
        {
            // Завантажуємо всі передані файли та повністю замінюємо колекцію
            entity.MediaFiles.Clear();
            foreach (var file in request.Files.Where(f => f is { Length: > 0 }))
            {
                var savedPath = await _storage.UploadAsync("posts", file, cancellationToken);
                entity.MediaFiles.Add(new PostMediaFileEntity
                {
                    FileName = savedPath
                });
            }

            // застосуємо текстові зміни (Slug/Content)
            PostMapper.ApplyUpdate(entity, new UpdatePostRequest
            {
                Slug = request.Slug,
                Content = request.Content,
                Files = null // файли вже обробили вручну
            });
        }

        await _repo.UpdateAsync(entity, cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetByIdAsync(id, cancellationToken);
        if (entity is null) return false;
        await _repo.DeleteAsync(entity, cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<SmallPostResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetWithMediaByIdAsync(id, cancellationToken);
        return entity is null ? null : PostMapper.ToSmallResponse(entity);
    }

    /// <inheritdoc />
    public async Task<SmallPostResponse?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var entity = await _repo.GetBySlugAsync(slug, cancellationToken);
        if (entity == null)
            return null;
        // Якщо отримали без медіа — доберемо повну версію для коректного мапінгу
        var withMedia = await _repo.GetWithMediaByIdAsync(entity.Id, cancellationToken) ?? entity;
        return PostMapper.ToSmallResponse(withMedia);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SmallPostResponse>> ListAsync(CancellationToken cancellationToken = default)
    {
        // Для простоти: забираємо всі пости з медіа через include string
        var list = await _repo.ListAsync(includeString: nameof(PostEntity.MediaFiles), cancellationToken: cancellationToken);
        return list.Select(PostMapper.ToSmallResponse).ToList();
    }
}
