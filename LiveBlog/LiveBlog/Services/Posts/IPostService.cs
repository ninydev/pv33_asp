using LiveBlog.Models.Posts;
using LiveBlog.Services.Base;

namespace LiveBlog.Services.Posts;

/// <summary>
/// Сервіс доменної логіки для роботи з дописами: валідація, бізнес-правила, оркестрація репозиторію.
/// </summary>
public interface IPostService : IService
{
    /// <summary>
    /// Створює новий допис. Перевіряє унікальність <c>Slug</c>.
    /// </summary>
    Task<SmallPostResponse> CreateAsync(CreatePostRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Оновлює існуючий допис. Повертає <c>false</c>, якщо допис не знайдено.
    /// </summary>
    Task<bool> UpdateAsync(int id, UpdatePostRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Видаляє допис. Повертає <c>false</c>, якщо не знайдено.
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Повертає коротку інформацію про допис за ідентифікатором.
    /// </summary>
    Task<SmallPostResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Повертає коротку інформацію про допис за <c>Slug</c>.
    /// </summary>
    Task<SmallPostResponse?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Повертає список дописів у вигляді коротких відповідей.
    /// </summary>
    Task<IReadOnlyList<SmallPostResponse>> ListAsync(CancellationToken cancellationToken = default);
}
