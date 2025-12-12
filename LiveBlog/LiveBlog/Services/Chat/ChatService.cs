using LiveBlog.Areas.Sse;
using LiveBlog.Models.Chat;
using LiveBlog.Models.IdentityUser;

namespace LiveBlog.Services.Chat;

public class ChatService
{
    private readonly SseService _sseService;
    private readonly AuthService _authService;
    private readonly ILogger<ChatService> _logger;
    
    public ChatService(SseService sseService,
        AuthService authService
        , ILogger<ChatService> logger)
    {
        _authService = authService;
        _sseService = sseService;
        _logger = logger;
    }

    public void SendMessage(ChatMessageRequest request)
    {
        MyIdentityUserEntity user = _authService.GetCurrentUserOrThrowAsync().Result;
        ChatMessageNotification notification = new ()
        {
            FromUserId = user.Id,
            FromUserName = user.UserName,
            Message = request.Message
        };

        _sseService.SendToAllAsync(notification);
        
        _logger.LogInformation($"New message: {request.Message}");
    }
    
    public void LeveChat()
    {
        
    }

    public void JoinChat()
    {
        
    }
}