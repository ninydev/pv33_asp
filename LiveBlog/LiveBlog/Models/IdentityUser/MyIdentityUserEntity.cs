using LiveBlog.Models.Comments;
using LiveBlog.Models.Likes;
using LiveBlog.Models.Posts;

namespace LiveBlog.Models.IdentityUser;

/// <summary>
/// Розширена сутність користувача ASP.NET Identity.
/// Містить навігаційні колекції до пов'язаних публікацій, вподобайок та коментарів.
/// </summary>
public class MyIdentityUserEntity : Microsoft.AspNetCore.Identity.IdentityUser
{
    /// <summary>
    /// Публікації, створені користувачем.
    /// </summary>
    public ICollection<PostEntity> Posts { get; set; } = new HashSet<PostEntity>();

    /// <summary>
    /// Вподобайки, які залишив користувач.
    /// </summary>
    public ICollection<PostLikeEntity> Likes { get; set; } = new HashSet<PostLikeEntity>();

    /// <summary>
    /// Коментарі, які залишив користувач.
    /// </summary>
    public ICollection<PostCommentEntity> Comments { get; set; } = new HashSet<PostCommentEntity>();
}