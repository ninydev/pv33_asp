using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Dto;

/// <summary>
///     DTO (Data Transfer Object) - об'єкт для передачі даних між шарами додатку.
///     Використовується для створення нового посту через API або форми.
///     Відмінності:
///     - Entity - представляє структуру даних в базі даних (включає все: ID, зв'язки, службові поля)
///     - DTO - спрощений об'єкт лише з потрібними полями для конкретної операції (створення, оновлення)
///     - ViewModel - об'єкт для відображення даних у View (може комбінувати дані з різних моделей для конкретної сторінки)
///     Приклад: PostCreateDto не містить Id (він ще не існує), але містить SelectedTagIds для зручної передачі з форми.
/// </summary>
public class PostCreateDto
{
    [Required]
    [MaxLength(256)]
    [Display(Name = "Title", Description = "Title of the post")]
    public string Title { get; set; }

    [Required]
    [MaxLength(256)]
    [Display(Name = "Slug", Description = "Title of the post")]
    public string Slug { get; set; }

    [Required]
    [MaxLength(2048)]
    [Display(Name = "Content", Description = "Content of the post")]
    public string Content { get; set; }

    public List<int> SelectedTagIds { get; set; } = new();
}