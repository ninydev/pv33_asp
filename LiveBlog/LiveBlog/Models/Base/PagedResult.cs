namespace LiveBlog.Models.Base;

/// <summary>
/// Результат пагінованого запиту: елементи сторінки та метадані.
/// </summary>
/// <typeparam name="T">Тип елементів.</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Елементи поточної сторінки.
    /// </summary>
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

    /// <summary>
    /// Загальна кількість елементів (без урахування skip/take).
    /// </summary>
    public int Total { get; init; }

    /// <summary>
    /// Номер сторінки (починаючи з 1).
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Розмір сторінки.
    /// </summary>
    public int PageSize { get; init; }
}
