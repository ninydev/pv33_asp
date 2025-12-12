using LiveBlog.Models.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveBlog.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ApiChatController : ControllerBase
{
    
    
    [HttpPost]
    [Route("send")]
    public IActionResult SendMessage(ChatMessageRequest request)
    {
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