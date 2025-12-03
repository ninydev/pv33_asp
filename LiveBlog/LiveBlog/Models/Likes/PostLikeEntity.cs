using System.ComponentModel.DataAnnotations.Schema;
using LiveBlog.Models.Base;
using LiveBlog.Models.Posts;

namespace LiveBlog.Models.Likes;

/// <summary>
/// Вподобайка (лайк) до публікації.
/// Успадковує <see cref="BaseEntity"/> для збереження автора лайку та часових міток.
/// </summary>
public class PostLikeEntity : BaseEntity
{
    /// <summary>
    /// Зовнішній ключ на публікацію, яку вподобали.
    /// </summary>
    [ForeignKey("Post")]  
    public int PostId { get; set; }

    /// <summary>
    /// Навігаційна властивість до публікації.
    /// </summary>
    public PostEntity Post { get; set; }
}