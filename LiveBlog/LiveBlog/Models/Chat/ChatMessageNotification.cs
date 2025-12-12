namespace LiveBlog.Models.Chat;

[Serializable]
public class ChatMessageNotification
{
    public string FromUserId { get; set; }
    public string FromUserName { get; set; }
    
    public string? ToUserId { get; set; }
    public string? ToUserName { get; set; }
    
    public string Message { get; set; }
}