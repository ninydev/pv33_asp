using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Entities;

public class MyIdentityUserEntity : IdentityUser
{
    // [JsonIgnore]
    public ICollection<PostEntity> Posts { get; set; } = new List<PostEntity>();
}