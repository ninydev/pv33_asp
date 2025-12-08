using LiveBlog.Data;
using LiveBlog.Models.Likes;
using LiveBlog.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace LiveBlog.Repositories.Likes;

public class PostLikeRepository : EfRepository<PostLikeEntity>, IPostLikeRepository
{
    public PostLikeRepository(ApplicationDbContext db) : base(db)
    {
    }

    public Task<PostLikeEntity?> GetByUserAndPostAsync(string userId, int postId, CancellationToken ct = default)
    {
        return Set.FirstOrDefaultAsync(x => x.UserId == userId && x.PostId == postId, ct);
    }

    public Task<bool> ExistsAsync(string userId, int postId, CancellationToken ct = default)
    {
        return Set.AnyAsync(x => x.UserId == userId && x.PostId == postId, ct);
    }

    public Task<int> CountByPostAsync(int postId, CancellationToken ct = default)
    {
        return Set.CountAsync(x => x.PostId == postId, ct);
    }
}
