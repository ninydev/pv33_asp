using LiveBlog.Services.Likes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LiveBlog.Controllers;

[ApiController]
[Route("api/posts/{postId:int}/likes")]
[Authorize]
public class PostLikesController : ControllerBase
{
    private readonly ILikeService _likes;

    public PostLikesController(ILikeService likes)
    {
        _likes = likes;
    }

    [HttpPost("toggle")]
    public async Task<IActionResult> Toggle(int postId, CancellationToken ct)
    {
        var (liked, count) = await _likes.ToggleAsync(postId, ct);
        return Ok(new { liked, count });
    }

    [HttpGet("status")]
    public async Task<IActionResult> Status(int postId, CancellationToken ct)
    {
        var liked = await _likes.IsLikedByMeAsync(postId, ct);
        return Ok(new { liked });
    }

    [AllowAnonymous]
    [HttpGet("count")]
    public async Task<IActionResult> Count(int postId, CancellationToken ct)
    {
        var count = await _likes.GetLikesCountAsync(postId, ct);
        return Ok(new { count });
    }
}
