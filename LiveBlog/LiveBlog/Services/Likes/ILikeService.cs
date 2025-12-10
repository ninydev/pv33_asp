using LiveBlog.Models.Likes;

namespace LiveBlog.Services.Likes;

public interface ILikeService
{
    /// Переключає лайк поточного користувача: якщо не було — поставить, якщо був — прибере.
    /// Повертає поточний стан після операції (true = лайк стоїть) і підсумкову кількість лайків.
    Task<LikePostNotification> ToggleAsync(int postId, CancellationToken ct = default);

    /// Чи лайкнув поточний користувач пост.
    Task<bool> IsLikedByMeAsync(int postId, CancellationToken ct = default);

    /// Поточна кількість лайків на пості.
    Task<int> GetLikesCountAsync(int postId, CancellationToken ct = default);
}
