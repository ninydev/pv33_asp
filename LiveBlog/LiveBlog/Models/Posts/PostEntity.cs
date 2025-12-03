using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LiveBlog.Models.Base;
using LiveBlog.Models.Media;

namespace LiveBlog.Models.Posts;

/// <summary>
/// Публікація (допис) у блозі.
/// Успадковує <see cref="BaseEntity"/>: має автора (<c>UserId</c>), часові мітки та ідентифікатор.
/// </summary>
public class PostEntity : BaseEntity
{
    /// <summary>
    /// Людиночитний унікальний ідентифікатор допису (slug), використовується в URL.
    /// </summary>
    [Required]
    [MaxLength(256)]
    public string Slug { get; set; }

    /// <summary>
    /// Основний контент допису (текст), максимум 2048 символів.
    /// </summary>
    [Required]
    [MaxLength(2048)]
    public string Content { get; set; }
    
    /// <summary>
    /// Колекція медіа-файлів, прикріплених до допису.
    /// </summary>
    public ICollection<PostMediaFileEntity> MediaFiles { get; set; } = new HashSet<PostMediaFileEntity>();
}