using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LiveBlog.Models.IdentityUser;

namespace LiveBlog.Models.Base;

public class BaseEntity
{
    [Key] public int Id { get; set; }
    
    [ForeignKey("CreatedBy")] public string UserId { get; set; }
    
    public MyIdentityUserEntity CreatedBy { get; set; }
    
    public BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? UpdatedAt { get; set; }
}