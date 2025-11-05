using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Entities;

public class MyIdentityUser : IdentityUser
{
    public ICollection<PostEntity> Posts { get; set; } = new List<PostEntity>();
}