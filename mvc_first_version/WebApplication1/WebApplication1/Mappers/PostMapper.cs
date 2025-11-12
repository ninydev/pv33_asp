using WebApplication1.Dto;
using WebApplication1.Entities;
using WebApplication1.ViewModel;

namespace WebApplication1.Mappers;

/// <summary>
///     PostMapper — спеціалізований клас для конвертації між різними представленнями поста.
///     Призначення:
///     - DTO -> Entity: під час створення/оновлення з форми або API у сутність БД.
///     - Entity -> ViewModel: для відображення на сторінках (Views) у зручному для UI форматі.
///     ВАЖЛИВО: Mapper не виконує доступ до БД і не підвантажує навігаційні властивості.
///     Будь-які операції, що потребують DbContext (наприклад, завантаження тегів за списком ідентифікаторів),
///     мають виконуватись у контролері або доменному сервісі, а не в мапері.
/// </summary>
public static class PostMapper
{
    /// <summary>
    ///     Перетворює PostCreateDto у PostEntity для збереження в БД.
    /// </summary>
    /// <param name="dto">Вхідні дані з форми створення поста</param>
    /// <param name="authorId">Ідентифікатор поточного користувача-автора</param>
    /// <returns>Новий екземпляр PostEntity</returns>
    public static PostEntity ToEntity(PostCreateDto dto, string authorId)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(authorId)) throw new ArgumentException("AuthorId is required", nameof(authorId));

        return new PostEntity
        {
            Title = dto.Title?.Trim(),
            Slug = dto.Slug?.Trim(),
            Content = dto.Content?.Trim(),
            AuthorId = authorId
            // CreatedAt ініціалізується у конструкторі PostEntity або може бути заповнений БД
        };
    }

    public static PostEntity ToEntity(PostUpdateDto dto, string authorId)
    {
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(authorId)) throw new ArgumentException("AuthorId is required", nameof(authorId));

        return new PostEntity
        {
            Title = dto.Title?.Trim(),
            Slug = dto.Slug?.Trim(),
            Content = dto.Content?.Trim(),
            AuthorId = authorId
            // CreatedAt ініціалізується у конструкторі PostEntity або може бути заповнений БД
        };
    }

    /// <summary>
    ///     Перетворює PostEntity у PostViewModel для відображення у представленнях.
    /// </summary>
    /// <remarks>
    ///     Щоб уникнути циклічних залежностей під час серіалізації (Post -> Author -> Posts -> ...),
    ///     мапимо навігаційні властивості у спрощені Show-моделі (Author, Tags).
    ///     Повертаємо лише значущі дані для UI: Id, Title, Slug, Content, Author(Id, UserName), Tags(Id, Name, Slug).
    /// </remarks>
    public static PostViewModel ToViewModel(PostEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        return new PostViewModel
        {
            Id = entity.Id,
            Title = entity.Title,
            Slug = entity.Slug,
            Content = entity.Content,
            Author = entity.Author?.ToShowViewModel(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            Tags = entity.Tags?.ToShowViewModels().ToList() ?? new List<ShortTagViewModel>()
        };
    }

    /// <summary>
    ///     Застосовує зміни з PostUpdateDto до наявної сутності PostEntity.
    /// </summary>
    /// <remarks>
    ///     - Не змінюємо авторство та службові поля (AuthorId, CreatedAt, UpdatedAt тут не встановлюємо вручну).
    ///     - Робота з тегами (many-to-many) має виконуватись у сервісі/контролері, де доступний DbContext.
    /// </remarks>
    public static void ApplyUpdates(PostEntity entity, PostUpdateDto dto)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        if (dto == null) throw new ArgumentNullException(nameof(dto));
        if (entity.Id != dto.Id) throw new ArgumentException("Id сутності та DTO не збігаються");

        entity.Title = dto.Title?.Trim();
        entity.Slug = dto.Slug?.Trim();
        entity.Content = dto.Content?.Trim();
        // UpdatedAt як правило виставляє БД або трекінг EF, залишаємо це поза мапером
        // entity.UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    ///     Створює PostUpdateDto з сутності для попереднього заповнення форми редагування.
    /// </summary>
    public static PostUpdateDto ToUpdateDto(PostEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        return new PostUpdateDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Slug = entity.Slug,
            Content = entity.Content,
            SelectedTagIds = entity.Tags?.Select(t => t.Id).ToList() ?? new List<int>()
        };
    }

    /// <summary>
    ///     Мапінг колекції сутностей у колекцію в’юмоделей.
    /// </summary>
    public static IEnumerable<PostViewModel> ToViewModels(IEnumerable<PostEntity> entities)
    {
        if (entities == null) yield break;
        foreach (var e in entities) yield return ToViewModel(e);
    }
}