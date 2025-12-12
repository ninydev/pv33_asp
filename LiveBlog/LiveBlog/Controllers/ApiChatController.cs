using LiveBlog.Models.Chat;
using LiveBlog.Services.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveBlog.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ApiChatController : ControllerBase
{
    private readonly ChatService _chatService;
    private readonly ILogger<ApiChatController> _logger;
    
    public ApiChatController(ChatService chatService, ILogger<ApiChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }
    
    [HttpPost]
    [Route("send")]
    public IActionResult SendMessage(ChatMessageRequest request)
    {
        _chatService.SendMessage(request);
        return Ok();
    }
    
    [HttpPost]
    [Route("leave")]
    public IActionResult LeaveChat()
    {
        return Ok();
    }

    [HttpPost]
    [Route("join")]
    public IActionResult JoinChat()
    {
        return Ok();
    }
}