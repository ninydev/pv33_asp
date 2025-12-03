using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LiveBlog.Models.Posts;

namespace LiveBlog.Models.Comments;

public class PostCommentEntity
{
    [ForeignKey("Post")]  public int PostId { get; set; }
    public PostEntity Post { get; set; }
    
    [Required] [MaxLength(2048)] public string Content { get; set; }
    
}