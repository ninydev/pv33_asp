using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LiveBlog.Models.Base;
using LiveBlog.Models.Media;

namespace LiveBlog.Models.Posts;

public class PostEntity : BaseEntity
{
    [Required] [MaxLength(256)] public string Slug { get; set; }

    [Required] [MaxLength(2048)] public string Content { get; set; }
    
    public ICollection<PostMediaFileEntity> MediaFiles { get; set; } = new HashSet<PostMediaFileEntity>();
}