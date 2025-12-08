using LiveBlog.Models.Posts;
using LiveBlog.Repositories.Base;
using LiveBlog.Models.Base;

namespace LiveBlog.Repositories.Posts;

/// <summary>
/// Репозиторій для роботи з дописами (<see cref="PostEntity"/>).
/// Виносить специфічні для постів запити.
/// </summary>
public interface IPostRepository : IRepository<PostEntity>
{
    /// <summary>
    /// Повертає допис за унікальним слагом або <c>null</c>, якщо не знайдено.
    /// </summary>
    Task<PostEntity?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Повертає допис з пов'язаними медіафайлами за ідентифікатором або <c>null</c>.
    /// </summary>
    Task<PostEntity?> GetWithMediaByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Повертає список постів з урахуванням пагінації, сортування, фільтрації та пошуку.
    /// </summary>
    Task<PagedResult<PostEntity>> ListAsync(
        PagedSortedFilteredRequest<PostSort, PostFilter> request,
        IEnumerable<string>? includes = null,
        CancellationToken cancellationToken = default);
}
