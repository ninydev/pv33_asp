namespace LiveBlog.Models.Chat;


public class ChatMessageRequest
{
    public string? ToUserId { get; set; }
    public string Message { get; set; }
}