using System.ComponentModel.DataAnnotations.Schema;
using LiveBlog.Models.Base;
using LiveBlog.Models.Posts;

namespace LiveBlog.Models.Media;

/// <summary>
/// Медіа-файл, прикріплений до публікації (зображення, відео тощо).
/// Успадковує <see cref="BaseEntity"/>, тому зберігає автора додавання та часові мітки.
/// </summary>
public class PostMediaFileEntity: BaseEntity
{
    /// <summary>
    /// Зовнішній ключ на публікацію, до якої прикріплено файл.
    /// </summary>
    [ForeignKey("Post")] 
    public int PostId { get; set; }

    /// <summary>
    /// Навігаційна властивість до публікації.
    /// </summary>
    public PostEntity Post { get; set; }
    
    /// <summary>
    /// Назва файлу (ім'я у файловій системі або сховищі).
    /// </summary>
    public string? FileName { get; set; }
}