using LiveBlog.Areas.Sse;
using LiveBlog.Models.Chat;
using LiveBlog.Models.IdentityUser;

namespace LiveBlog.Services.Chat;

public class ChatService
{
    private readonly SseService _sseService;
    private readonly ILogger<ChatService> _logger;
    
    public ChatService(SseService sseService
        , ILogger<ChatService> logger)
    {
        _sseService = sseService;
        _logger = logger;
    }

    public ChatMessageNotification BuildMessage(string FromUserId, string FromUserName, ChatMessageRequest request)
    {
        return new ChatMessageNotification()
        {
            FromUserId = FromUserId,
            FromUserName = FromUserName,
            Message = request.Message
        };
    }

    public void SendMessage(ChatMessageNotification notification)
    {

        _sseService.SendToAllAsync(notification);
        
        _logger.LogInformation($"New message: {notification.Message}");
    }
    
    public void LeveChat()
    {
        
    }

    public void JoinChat()
    {
        
    }
}