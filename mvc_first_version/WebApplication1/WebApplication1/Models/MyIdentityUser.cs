using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models;

public class MyIdentityUser : IdentityUser
{
    public ICollection<PostModel> Posts { get; set; } = new List<PostModel>();
}