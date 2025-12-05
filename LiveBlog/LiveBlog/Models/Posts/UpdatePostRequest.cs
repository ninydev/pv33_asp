using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LiveBlog.Models.Posts;

/// <summary>
/// Запит на оновлення існуючого допису.
/// Поля є необов'язковими: вказуйте лише те, що потрібно змінити.
/// Колекція файлів приймається з форми; у БД зберігатимуться лише імена файлів.
/// </summary>
public class UpdatePostRequest
{
    /// <summary>
    /// Новий slug (за потреби). Якщо <c>null</c> — не змінюється.
    /// </summary>
    [MaxLength(256)]
    public string? Slug { get; set; }

    /// <summary>
    /// Новий текст допису (за потреби). Якщо <c>null</c> — не змінюється.
    /// </summary>
    [MaxLength(2048)]
    public string? Content { get; set; }

    /// <summary>
    /// Набір нових файлів (зображення або відео). Якщо <c>null</c> — вкладення не змінюються.
    /// Якщо передано порожній список — вкладення буде очищено.
    /// </summary>
    public IList<IFormFile>? Files { get; set; }
}
