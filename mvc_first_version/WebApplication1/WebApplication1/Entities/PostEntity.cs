using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace WebApplication1.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Представляє сутність посту в базі даних.
/// 
/// Суфікс "Entity" вказує на те, що це клас предметної області (domain entity),
/// який безпосередньо відображається на таблицю в базі даних через ORM (Entity Framework).
/// 
/// Різниця між Entity та Model:
/// - Entity (Сутність) - це об'єкт, який має унікальний ідентифікатор (Id) і безпосередньо 
///   зберігається в базі даних. Містить анотації для маппінгу БД ([Key], [Required], [ForeignKey]).
///   Представляє структуру даних на рівні персистентності (persistence layer).
/// 
/// - Model (Модель) - це об'єкт для передачі даних між шарами додатку (DTO, ViewModel).
///   Використовується для відображення даних у представленні (View) або API responses.
///   Не містить логіки роботи з БД і може об'єднувати дані з кількох Entity.
/// 
/// Приклад:
/// - PostEntity зберігається в БД і містить всі поля таблиці
/// - PostViewModel може містити лише Title, Content та ім'я автора для відображення
/// - CreatePostDto може містити лише Title, Content та ім'я автора для відображення
/// - CreatePostModel може містити лише поля, необхідні для створення нового посту
/// 
/// Така розбивка дозволяє:
/// 1. Відокремити логіку персистентності від логіки представлення
/// 2. Уникнути over-posting атак (коли користувач змінює поля, які не повинен)
/// 3. Мати різні контракти для різних операцій (створення, читання, оновлення)
/// 4. Легше рефакторити БД без впливу на API контракти
/// </summary>
public class PostEntity
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(256)]
    public string Title { get; set; }
    
    [Required]
    [MaxLength(256)]
    public string Slug { get; set; }
    
    [Required]
    [MaxLength(2048)]
    public string Content { get; set; }
    
    [ForeignKey("Author")]
    public string AuthorId { get; set; }
    public MyIdentityUser Author { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? UpdatedAt { get; set; }
    
    public ICollection<TagModel> Tags { get; set; } = new List<TagModel>();

    public PostEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }
    
    
}