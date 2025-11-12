using System.ComponentModel.DataAnnotations;

namespace WebApplication1.ViewModel;

/// <summary>
///     ViewModel (модель подання) — це тип класів, призначений для відображення даних у представленнях (Views).
///     Він формується спеціально під потреби UI та може поєднувати дані з різних джерел.
///     Відмінності від інших типів моделей:
///     - Entity (сутність БД): описує структуру таблиці та правила збереження даних, містить атрибути для мапінгу БД (Id,
///     ForeignKey тощо).
///     - DTO (об’єкт передачі даних): використовується для приймання/передачі даних через API або форми (наприклад,
///     створення поста).
///     - ViewModel: орієнтований на читання/відображення. Може мати зручні для UI поля, форматування дат, готові до
///     рендеру значення.
///     Цей PostViewModel віддається на сторінки для показу публікацій: заголовок, вміст, автор, дата створення/оновлення
///     та теги.
/// </summary>
public class PostViewModel
{
    [ScaffoldColumn(false)] public int Id { get; set; }

    [Display(Name = "Заголовок", Description = "Заголовок публікації")]
    [MaxLength(256)]
    public string Title { get; set; }

    [Display(Name = "Слаг", Description = "Людинозрозумілий ідентифікатор у URL")]
    [MaxLength(256)]
    public string Slug { get; set; }

    [Display(Name = "Вміст", Description = "Основний текст публікації")]
    [DataType(DataType.MultilineText)]
    [MaxLength(2048)]
    public string Content { get; set; }

    [Display(Name = "Автор", Description = "Користувач, який створив публікацію")]
    public ShortUserViewModel Author { get; set; }

    [Display(Name = "Створено", Description = "Мітка часу створення запису")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = false)]
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Оновлено", Description = "Мітка часу останнього оновлення запису (якщо було)")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", NullDisplayText = "—", ApplyFormatInEditMode = false)]
    public DateTime? UpdatedAt { get; set; }

    [Display(Name = "Теги", Description = "Список тегів, прив’язаних до публікації")]
    public ICollection<ShortTagViewModel> Tags { get; set; } = new List<ShortTagViewModel>();
}