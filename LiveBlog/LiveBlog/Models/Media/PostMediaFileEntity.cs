using System.ComponentModel.DataAnnotations.Schema;
using LiveBlog.Models.Base;
using LiveBlog.Models.Posts;

namespace LiveBlog.Models.Media;

public class PostMediaFileEntity: BaseEntity
{
    [ForeignKey("Post")]public int PostId { get; set; }
    public PostEntity Post { get; set; }
    
    public string? FileName { get; set; }
}