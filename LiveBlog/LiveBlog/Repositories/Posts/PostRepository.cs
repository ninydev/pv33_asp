using LiveBlog.Data;
using LiveBlog.Models.Posts;
using LiveBlog.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace LiveBlog.Repositories.Posts;

/// <summary>
/// Реалізація репозиторію для роботи з дописами.
/// Інкапсулює специфічні запити до EF Core для сутності <see cref="PostEntity"/>.
/// </summary>
public class PostRepository : EfRepository<PostEntity>, IPostRepository
{
    public PostRepository(ApplicationDbContext db) : base(db)
    {
    }

    /// <inheritdoc />
    public Task<PostEntity?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return Set.AsNoTracking().FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PostEntity?> GetWithMediaByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return Set.Include(p => p.MediaFiles).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
