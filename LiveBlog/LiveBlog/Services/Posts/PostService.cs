using LiveBlog.Models.Posts;
using LiveBlog.Repositories.Posts;

namespace LiveBlog.Services.Posts;

/// <summary>
/// Реалізація сервісу доменної логіки для роботи з дописами.
/// Відповідає за валідацію та делегує доступ до даних репозиторію.
/// </summary>
public class PostService : IPostService
{
    private readonly IPostRepository _repo;

    public PostService(IPostRepository repo)
    {
        _repo = repo;
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

        PostMapper.ApplyUpdate(entity, request);

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
