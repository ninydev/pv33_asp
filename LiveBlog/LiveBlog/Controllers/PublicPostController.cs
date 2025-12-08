using LiveBlog.Models.Base;
using LiveBlog.Models.Posts;
using LiveBlog.Services;
using LiveBlog.Services.Posts;
using Microsoft.AspNetCore.Mvc;

namespace LiveBlog.Controllers;

public class PublicPostController : Controller
{
    private readonly IPostService _postService;

    public PublicPostController(IPostService postService, AuthService authService)
    {
        _postService = postService;
    }

    /// <summary>
    /// Список постів з підтримкою пагінації, сортування та фільтрації.
    /// Приймає комплексний запит напряму із query-рядка.
    /// Приклади:
    ///  - ?Page=1&PageSize=10&SortBy=Id&SortDirection=Desc
    ///  - ?Filter.Query=aspnet&SortBy=CreatedAt&SortDirection=Desc
    ///  - ?Filter.UserId=123&Filter.DateFrom=2025-01-01&Filter.DateTo=2025-12-31
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PagedSortedFilteredRequest<PostSort, PostFilter> request,
        CancellationToken ct = default)
    {
        return View(await _postService.ListAsync(request, ct));
    }

}