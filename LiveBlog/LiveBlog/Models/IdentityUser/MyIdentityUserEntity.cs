using LiveBlog.Models.Comments;
using LiveBlog.Models.Likes;
using LiveBlog.Models.Posts;

namespace LiveBlog.Models.IdentityUser;

public class MyIdentityUserEntity : Microsoft.AspNetCore.Identity.IdentityUser
{
    public ICollection<PostEntity> Posts { get; set; } = new HashSet<PostEntity>();
    public ICollection<PostLikeEntity> Likes { get; set; } = new HashSet<PostLikeEntity>();
    public ICollection<PostCommentEntity> Comments { get; set; } = new HashSet<PostCommentEntity>();
}