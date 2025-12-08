using LiveBlog.Models.Likes;
using LiveBlog.Repositories.Base;

namespace LiveBlog.Repositories.Likes;

public interface IPostLikeRepository : IRepository<PostLikeEntity>
{
    Task<PostLikeEntity?> GetByUserAndPostAsync(string userId, int postId, CancellationToken ct = default);
    Task<bool> ExistsAsync(string userId, int postId, CancellationToken ct = default);
    Task<int> CountByPostAsync(int postId, CancellationToken ct = default);
}
