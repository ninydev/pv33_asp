using LiveBlog.Models.Posts;
using LiveBlog.Services.Posts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LiveBlog.Models.Base;

namespace LiveBlog.Controllers;

/// <summary>
/// Контролер для керування дописами поточного користувача (CRUD).
/// Повертає представлення (Razor Views) для кожної дії.
/// </summary>
[Authorize]
public class UserPostController : Controller
{
    private readonly IPostService _postService;

    public UserPostController(IPostService postService)
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
    public async Task<IActionResult> Index([FromQuery] PagedSortedFilteredRequest<PostSort, PostFilter> request, CancellationToken ct = default)
    {
        var result = await _postService.ListAsync(request, ct);
        return View(result);
    }

    /// <summary>
    /// Деталі допису за ідентифікатором.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var post = await _postService.GetByIdAsync(id, ct);
        if (post == null) return NotFound();
        return View(post);
    }

    /// <summary>
    /// Деталі допису за slug.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> DetailsBySlug(string slug, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(slug)) return BadRequest();
        var post = await _postService.GetBySlugAsync(slug, ct);
        if (post == null) return NotFound();
        return View("Details", post);
    }

    /// <summary>
    /// Форма створення нового допису (GET).
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Обробник створення нового допису (POST).
    /// Перевіряє валідацію моделі та унікальність slug на рівні сервісу.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePostRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        try
        {
            var created = await _postService.CreateAsync(request, ct);
            return RedirectToAction(nameof(Details), new { id = created.Id });
        }
        catch (Exception ex)
        {
            // Додаємо помилку в модель і повертаємо форму
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
    }

    /// <summary>
    /// Форма редагування допису (GET).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var post = await _postService.GetByIdAsync(id, ct);
        if (post == null) return NotFound();

        // Заповнимо модель оновлення базовими значеннями
        var model = new UpdatePostRequest
        {
            Slug = post.Slug,
            Content = post.Content
        };
        return View(model);
    }

    /// <summary>
    /// Обробник оновлення допису (POST).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdatePostRequest request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(request);
        }

        try
        {
            var ok = await _postService.UpdateAsync(id, request, ct);
            if (!ok) return NotFound();
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }
    }

    /// <summary>
    /// Підтвердження видалення допису (GET).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var post = await _postService.GetByIdAsync(id, ct);
        if (post == null) return NotFound();
        return View(post);
    }

    /// <summary>
    /// Видалення допису (POST). За вимогою — залишаємо тіло методу порожнім.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        // Навмисно порожньо: реалізацію можна додати пізніше
        // await _postService.DeleteAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }
}