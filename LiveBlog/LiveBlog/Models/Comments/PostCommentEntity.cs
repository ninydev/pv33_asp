using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LiveBlog.Models.Base;
using LiveBlog.Models.Posts;

namespace LiveBlog.Models.Comments;

/// <summary>
/// Коментар до публікації.
/// Прив'язаний до конкретного допису через <see cref="PostId"/>.
/// Зверніть увагу: наразі сутність не успадковує <c>BaseEntity</c>, тому не має автора та часових міток.
/// Якщо потрібні авторство/модерація/час створення — розгляньте успадкування від <c>BaseEntity</c>.
/// </summary>
public class PostCommentEntity : BaseEntity
{
    /// <summary>
    /// Зовнішній ключ на допис, до якого належить коментар.
    /// </summary>
    [ForeignKey("Post")]  
    public int PostId { get; set; }
    
    /// <summary>
    /// Навігаційна властивість до пов'язаного допису.
    /// </summary>
    public PostEntity Post { get; set; }
    
    /// <summary>
    /// Текст коментаря (до 2048 символів).
    /// </summary>
    [Required]
    [MaxLength(2048)] 
    public string Content { get; set; }
}