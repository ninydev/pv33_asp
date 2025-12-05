namespace LiveBlog.Models.Posts;

/// <summary>
/// Коротка відповідь про допис, що використовується у списках/попередньому перегляді.
/// Містить базову інформацію та перелік імен прикріплених файлів.
/// </summary>
public class SmallPostResponse
{
    /// <summary>
    /// Ідентифікатор допису.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Унікальний slug для URL.
    /// </summary>
    public string Slug { get; set; }

    /// <summary>
    /// Текст допису (скорочений/повний залежно від сценарію використання).
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Імена медіа-файлів, прикріплених до допису.
    /// </summary>
    public IList<string> FileNames { get; set; } = new List<string>();
}
