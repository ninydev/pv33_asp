using System.ComponentModel.DataAnnotations.Schema;
using LiveBlog.Models.Base;
using LiveBlog.Models.Posts;

namespace LiveBlog.Models.Likes;

public class PostLikeEntity : BaseEntity
{
    [ForeignKey("Post")]  public int PostId { get; set; }
    public PostEntity Post { get; set; }
}