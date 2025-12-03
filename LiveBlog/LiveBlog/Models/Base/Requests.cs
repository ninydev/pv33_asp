using System.ComponentModel.DataAnnotations;

namespace LiveBlog.Models.Base;

/// <summary>
/// Напрям сортування: за зростанням або спаданням.
/// </summary>
public enum SortDirection
{
    /// <summary>
    /// Сортування за зростанням (ASC).
    /// </summary>
    Asc = 0,

    /// <summary>
    /// Сортування за спаданням (DESC).
    /// </summary>
    Desc = 1
}

/// <summary>
/// Контракт для запитів з підтримкою пагінації.
/// </summary>
public interface IPageableRequest
{
    /// <summary>
    /// Номер сторінки (починаючи з 1).
    /// </summary>
    int Page { get; }

    /// <summary>
    /// Розмір сторінки (кількість елементів на сторінці).
    /// </summary>
    int PageSize { get; }
}

/// <summary>
/// Контракт для запитів з підтримкою типізованого сортування.
/// </summary>
/// <typeparam name="TSort">Enum, що описує доступні поля для сортування.</typeparam>
public interface ISortableRequest<TSort> where TSort : struct, Enum
{
    /// <summary>
    /// Поле для сортування (enum), може бути відсутнім.
    /// </summary>
    TSort? SortBy { get; }

    /// <summary>
    /// Напрям сортування (за замовчуванням — Asc).
    /// </summary>
    SortDirection SortDirection { get; }
}

/// <summary>
/// Контракт для запитів з підтримкою фільтрації.
/// </summary>
/// <typeparam name="TFilter">Тип моделі фільтра (DTO), що описує критерії пошуку.</typeparam>
public interface IFilterableRequest<TFilter>
{
    /// <summary>
    /// Об'єкт фільтра з критеріями відбору; може бути відсутній.
    /// </summary>
    TFilter? Filter { get; }
}

/// <summary>
/// Базовий запит для пагінації.
/// Використовуйте як самостійно, так і як базовий тип для розширених запитів.
/// </summary>
public record PageableRequest : IPageableRequest
{
    /// <summary>
    /// Номер сторінки (мінімум 1). За замовчуванням 1.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Номер сторінки має бути 1 або більше.")]
    public int Page { get; init; } = 1;

    /// <summary>
    /// Розмір сторінки (мінімум 1, максимум 100). За замовчуванням 10.
    /// </summary>
    [Range(1, 100, ErrorMessage = "Розмір сторінки має бути в діапазоні 1..100.")]
    public int PageSize { get; init; } = 10;
}

/// <summary>
/// Запит із підтримкою типізованого сортування.
/// </summary>
/// <typeparam name="TSort">Enum з переліком полів для сортування.</typeparam>
public record SortableRequest<TSort> : ISortableRequest<TSort> where TSort : struct, Enum
{
    /// <summary>
    /// Поле для сортування (enum), може бути null — тоді застосовується сортування за умовчанням на боці сервісу/репозиторію.
    /// </summary>
    public TSort? SortBy { get; init; }

    /// <summary>
    /// Напрям сортування (Asc/Desc). За замовчуванням Asc.
    /// </summary>
    public SortDirection SortDirection { get; init; } = SortDirection.Asc;
}

/// <summary>
/// Запит із підтримкою фільтрації через окремий DTO об'єкт.
/// </summary>
/// <typeparam name="TFilter">Тип моделі фільтра (DTO).</typeparam>
public record FilterableRequest<TFilter> : IFilterableRequest<TFilter>
{
    /// <summary>
    /// Об'єкт фільтра з критеріями відбору; може бути null.
    /// </summary>
    public TFilter? Filter { get; init; }
}

/// <summary>
/// Комплексний запит, що поєднує пагінацію, типізоване сортування та фільтрацію.
/// Зручно використовувати у контролерах та сервісах для уніфікованої роботи із запитами списків.
/// </summary>
/// <typeparam name="TSort">Enum з переліком доступних полів для сортування.</typeparam>
/// <typeparam name="TFilter">Тип DTO для фільтра.</typeparam>
public record PagedSortedFilteredRequest<TSort, TFilter> : PageableRequest, ISortableRequest<TSort>, IFilterableRequest<TFilter>
    where TSort : struct, Enum
{
    /// <inheritdoc />
    public TSort? SortBy { get; init; }

    /// <inheritdoc />
    public SortDirection SortDirection { get; init; } = SortDirection.Asc;

    /// <inheritdoc />
    public TFilter? Filter { get; init; }
}
