using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Dto;
using WebApplication1.Mappers;
using WebApplication1.ViewModel;

namespace WebApplication1.Controllers;

/// <summary>
///     API контролер для роботи з публікаціями (пости користувачів).
///     Забезпечує CRUD-операції та повертає дані у вигляді ViewModel без циклічних залежностей.
/// </summary>
/// <remarks>
///     Дотримуємося найкращих практик:
///     - Чіткі коди відповідей і анотації [ProducesResponseType]
///     - AsNoTracking для операцій читання (покращує продуктивність)
///     - Валідація ModelState та перевірки авторства для змінних операцій
///     - CancellationToken для коректного скасування довгих запитів
/// </remarks>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class ApiUserPostController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ApiUserPostController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Отримати список всіх публікацій.
    /// </summary>
    /// <param name="ct">Токен скасування операції</param>
    /// <returns>Колекцію постів у вигляді PostViewModel</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<PostViewModel>))]
    public async Task<ActionResult<IEnumerable<PostViewModel>>> GetPosts(CancellationToken ct = default)
    {
        // AsNoTracking — швидше для читання, коли не плануємо змінювати сутності
        var query = _context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Tags);

        var list = await query.ToListAsync(ct);
        var vm = PostMapper.ToViewModels(list);
        return Ok(vm);
    }

    /// <summary>
    ///     Отримати публікацію за ідентифікатором.
    /// </summary>
    /// <param name="id">Ідентифікатор поста</param>
    /// <param name="ct">Токен скасування операції</param>
    /// <returns>Публікацію у вигляді PostViewModel або 404, якщо не знайдено</returns>
    [HttpGet("{id:int}", Name = nameof(GetPostEntity))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostViewModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PostViewModel>> GetPostEntity(int id, CancellationToken ct = default)
    {
        var postEntity = await _context.Posts
            .AsNoTracking()
            .Include(p => p.Author)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (postEntity == null) return NotFound();

        return Ok(PostMapper.ToViewModel(postEntity));
    }

    /// <summary>
    ///     Оновити існуючу публікацію.
    /// </summary>
    /// <param name="id">Ідентифікатор поста</param>
    /// <param name="data">Дані для оновлення</param>
    /// <param name="ct">Токен скасування операції</param>
    /// <returns>204 NoContent у разі успіху; 400/403/404 у разі помилок</returns>
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutPostEntity(int id, [FromBody] PostUpdateDto data,
        CancellationToken ct = default)
    {
        if (data == null) return BadRequest("Порожні дані запиту.");
        if (id != data.Id) return BadRequest("Id у маршруті і тілі не збігаються.");
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            ModelState.AddModelError(string.Empty, "Не вдалося визначити поточного користувача.");
            return BadRequest(ModelState);
        }

        var entity = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (entity == null) return NotFound();
        // Перевірка авторства: дозволяємо змінювати лише власнику
        if (!string.Equals(entity.AuthorId, userId, StringComparison.Ordinal)) return Forbid();

        PostMapper.ApplyUpdates(entity, data);

        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PostEntityExists(id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    /// <summary>
    ///     Створити нову публікацію для поточного користувача.
    /// </summary>
    /// <param name="data">Дані нового поста</param>
    /// <param name="ct">Токен скасування операції</param>
    /// <returns>Створений пост (201 Created) з Location заголовком</returns>
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PostViewModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PostViewModel>> PostPostEntity([FromBody] PostCreateDto data,
        CancellationToken ct = default)
    {
        if (data == null) return BadRequest("Порожні дані запиту.");
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            ModelState.AddModelError(string.Empty, "Не вдалося визначити поточного користувача.");
            return BadRequest(ModelState);
        }

        var postEntity = PostMapper.ToEntity(data, userId);
        _context.Posts.Add(postEntity);
        await _context.SaveChangesAsync(ct);

        // Повертаємо 201 Created із правильним посиланням на ресурс
        return CreatedAtAction(nameof(GetPostEntity), new { id = postEntity.Id }, PostMapper.ToViewModel(postEntity));
    }

    /// <summary>
    ///     Видалити публікацію за ідентифікатором.
    /// </summary>
    /// <param name="id">Ідентифікатор поста</param>
    /// <param name="ct">Токен скасування операції</param>
    /// <returns>204 NoContent у разі успіху; 403/404 у разі помилок</returns>
    [HttpDelete("{id:int}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePostEntity(int id, CancellationToken ct = default)
    {
        var postEntity = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (postEntity == null) return NotFound();

        var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            ModelState.AddModelError(string.Empty, "Не вдалося визначити поточного користувача.");
            return BadRequest(ModelState);
        }

        if (!string.Equals(postEntity.AuthorId, userId, StringComparison.Ordinal)) return Forbid();

        _context.Posts.Remove(postEntity);
        await _context.SaveChangesAsync(ct);

        return NoContent();
    }

    private bool PostEntityExists(int id)
    {
        return _context.Posts.Any(e => e.Id == id);
    }
}