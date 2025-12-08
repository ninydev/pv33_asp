using System.Data;
using LiveBlog.Models.Comments;
using LiveBlog.Models.Likes;
using LiveBlog.Models.Media;
using LiveBlog.Models.Posts;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LiveBlog.Data;

public class ApplicationDbContext : IdentityDbContext
{
    
    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<PostLikeEntity> Likes { get; set; }
    public DbSet<PostCommentEntity> Comments { get; set; }
    public DbSet<PostMediaFileEntity> MediaFiles { get; set; }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Встановлюємо унікальний індекс для поля Slug
        builder.Entity<PostEntity>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        // Унікальний індекс: один користувач може лайкнути конкретний пост лише раз
        builder.Entity<PostLikeEntity>()
            .HasIndex(l => new { l.PostId, l.UserId })
            .IsUnique();
    }
}