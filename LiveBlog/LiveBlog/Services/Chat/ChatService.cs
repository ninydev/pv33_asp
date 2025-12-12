using LiveBlog.Areas.Sse;
using LiveBlog.Models.Chat;

namespace LiveBlog.Services.Chat;

public class ChatService
{
    private readonly SseService _sseService;
    private readonly ILogger<ChatService> _logger;
    
    public ChatService(SseService sseService, ILogger<ChatService> logger)
    {
        _sseService = sseService;
        _logger = logger;
    }

    public void SendMessage(ChatMessageRequest request)
    {
        _logger.LogInformation($"New message: {request.Message}");
    }
    
    public void LeveChat()
    {
        
    }

    public void JoinChat()
    {
        
    }
}