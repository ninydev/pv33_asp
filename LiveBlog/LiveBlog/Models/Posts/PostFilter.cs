using System.ComponentModel.DataAnnotations;

namespace LiveBlog.Models.Posts;

/// <summary>
/// DTO фільтрації для списку постів.
/// </summary>
public class PostFilter
{
    /// <summary>
    /// Повернути лише пости конкретного користувача.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Загальний пошук: шукає у <see cref="PostEntity.Slug"/> та <see cref="PostEntity.Content"/> (contains, case-insensitive).
    /// </summary>
    public string? Query { get; init; }

    /// <summary>
    /// Пошук за частиною slug (contains, case-insensitive).
    /// </summary>
    [MaxLength(256)]
    public string? SlugContains { get; init; }

    /// <summary>
    /// Пошук за частиною контенту (contains, case-insensitive).
    /// </summary>
    [MaxLength(512)]
    public string? ContentContains { get; init; }

    /// <summary>
    /// Відфільтрувати записи, створені починаючи з цієї дати (UTC).
    /// </summary>
    public DateTime? DateFrom { get; init; }

    /// <summary>
    /// Відфільтрувати записи, створені до цієї дати включно (UTC).
    /// </summary>
    public DateTime? DateTo { get; init; }
}
