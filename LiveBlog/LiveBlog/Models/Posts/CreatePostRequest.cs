using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LiveBlog.Models.Posts;

/// <summary>
/// Запит на створення нового допису.
/// Приймає дані з HTML-форми: слаг, текст та колекцію файлів (зображення або відео).
/// У БД зберігаємо лише імена файлів.
/// </summary>
public class CreatePostRequest
{
    /// <summary>
    /// Ідентифікатор користувача (автора). Встановлюється контролером перед викликом сервісу.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Людиночитний унікальний ідентифікатор (slug) для URL.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Slug { get; set; }

    /// <summary>
    /// Основний текст допису.
    /// </summary>
    [Required]
    [MaxLength(2048)]
    public string Content { get; set; }

    /// <summary>
    /// Колекція завантажених файлів (зображення або відео), що приймаються з форми.
    /// Зберігаємо лише їх назви у БД.
    /// </summary>
    public IList<IFormFile>? Files { get; set; }
}
