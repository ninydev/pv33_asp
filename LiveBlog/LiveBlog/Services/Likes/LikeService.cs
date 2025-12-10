using LiveBlog.Data;
using LiveBlog.Models.Likes;
using LiveBlog.Repositories.Likes;
using LiveBlog.Repositories.Posts;
using Microsoft.EntityFrameworkCore;

namespace LiveBlog.Services.Likes;

/// <summary>
/// Сервіс доменної логіки для лайків постів. Інкапсулює усю бізнес-логіку.
/// </summary>
public class LikeService : ILikeService
{
    private readonly IPostLikeRepository _likes;
    private readonly IPostRepository _posts;
    private readonly AuthService _auth;
    private readonly ApplicationDbContext _db;

    public LikeService(IPostLikeRepository likes, IPostRepository posts, AuthService auth, ApplicationDbContext db)
    {
        _likes = likes;
        _posts = posts;
        _auth = auth;
        _db = db;
    }

    public async Task<LikePostNotification> ToggleAsync(int postId, CancellationToken ct = default)
    {
        var notification = new LikePostNotification();
        
        var userId = _auth.GetCurrentUserIdOrThrow();

        var post = await _posts.GetByIdAsync(postId, ct);
        if (post is null)
            throw new KeyNotFoundException("Пост не знайдено");

        notification.PostId = postId;
        notification.UserId = userId;
        
        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var existing = await _likes.GetByUserAndPostAsync(userId, postId, ct);
        if (existing is null)
        {
            // Ставимо лайк
            await _likes.AddAsync(new PostLikeEntity { PostId = postId, UserId = userId }, ct);

            // Оновлюємо лічильник розрахунком із джерела істини (таблиці лайків), щоб уникнути гонок
            post.LikesCount = await _likes.CountByPostAsync(postId, ct);
            await _posts.UpdateAsync(post, ct);

            await tx.CommitAsync(ct);
            
            notification.IsLiked = true;
            notification.LikesCount = post.LikesCount;
            return notification;
        }
        else
        {
            // Знімаємо лайк
            await _likes.DeleteAsync(existing, ct);
            post.LikesCount = await _likes.CountByPostAsync(postId, ct);
            await _posts.UpdateAsync(post, ct);

            await tx.CommitAsync(ct);
            
            notification.IsLiked = false;
            notification.LikesCount = post.LikesCount;
            return notification;
        }
    }

    public async Task<bool> IsLikedByMeAsync(int postId, CancellationToken ct = default)
    {
        var userId = _auth.GetCurrentUserIdOrThrow();
        return await _likes.ExistsAsync(userId, postId, ct);
    }

    public async Task<int> GetLikesCountAsync(int postId, CancellationToken ct = default)
    {
        var post = await _posts.GetByIdAsync(postId, ct);
        if (post is null) throw new KeyNotFoundException("Пост не знайдено");
        return post.LikesCount;
    }
}
