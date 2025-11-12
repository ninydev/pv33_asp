using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dto;

/// <summary>
///     PostUpdateDto — DTO для операції оновлення публікації.
///     Чому окремий DTO для оновлення — це краще:
///     - Захист від over-posting: у контракті присутні лише ті поля, які ми дозволяємо змінювати з форми/API.
///     - Ясні контракти: різні сценарії (Create/Update) можуть мати різні вимоги, валідацію та склад полів.
///     - SRP і тестованість: DTO не містить логіки доступу до БД і зручний для валідації/моків.
///     - Еволюційність API: зміна полів зберігання (у Entity) не ламає контракти оновлення.
///     Зауваження:
///     - Ідентифікатор Id обов’язковий для цільового оновлення конкретного запису.
///     - Колекція SelectedTagIds передається окремо; фактичне завантаження Tag-сутностей має виконуватися у
///     сервісі/контролері,
///     а не в мапері, щоб дотримуватися принципу єдиної відповідальності.
/// </summary>
public class PostUpdateDto
{
    [Required] public int Id { get; set; }

    [Required]
    [StringLength(256, MinimumLength = 3, ErrorMessage = "Заголовок має містити від 3 до 256 символів")]
    [Display(Name = "Заголовок", Description = "Заголовок публікації")]
    public string Title { get; set; }

    [Required]
    [StringLength(256, MinimumLength = 3, ErrorMessage = "Слаг має містити від 3 до 256 символів")]
    [RegularExpression(@"^[a-z0-9\-]+$",
        ErrorMessage = "Slug може містити лише малі латинські літери, цифри та дефіси")]
    [Display(Name = "Слаг", Description = "URL-ідентифікатор (лише малі літери, цифри та дефіси)")]
    public string Slug { get; set; }

    [Required]
    [StringLength(2048, MinimumLength = 1, ErrorMessage = "Вміст не може бути порожнім і має бути до 2048 символів")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Вміст", Description = "Основний текст публікації")]
    public string Content { get; set; }

    /// <summary>
    ///     Перелік вибраних тегів для оновлення зв’язків many-to-many.
    ///     Примітка: завантаження/звірка фактичних сутностей Tag має виконуватися в сервісі/контролері.
    /// </summary>
    [Display(Name = "Теги", Description = "Список ідентифікаторів вибраних тегів")]
    public List<int> SelectedTagIds { get; set; } = new();
}