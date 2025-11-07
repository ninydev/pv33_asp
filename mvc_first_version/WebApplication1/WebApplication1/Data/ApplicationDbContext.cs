using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1.Data;

public class ApplicationDbContext : IdentityDbContext<MyIdentityUserEntity>
{
    public DbSet<TagEntity> Tags { get; set; }
    public DbSet<BookModel> Books { get; set; }
    
    public DbSet<PostEntity> Posts { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}