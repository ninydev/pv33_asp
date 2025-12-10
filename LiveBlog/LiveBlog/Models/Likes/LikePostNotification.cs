namespace LiveBlog.Models.Likes;

using System.Text.Json.Serialization;

[Serializable]
public class LikePostNotification

{
    public string UserId { get; set; }
    public string? UserName { get; set; }
    
    public int PostId { get; set; }
    public string AuthorId { get; set; }
    public int LikesCount { get; set; }
    
    public bool IsLiked { get; set; }
}