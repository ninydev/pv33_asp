using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Dto;
using WebApplication1.Mappers;

namespace WebApplication1.Controllers;

/// <summary>
///     MVC-контролер для роботи з публікаціями користувачів: перегляд, створення, редагування та видалення.
///     Дотримується кращих практик ASP.NET MVC: розділення відповідальностей, захист від over-posting, валідація моделі,
///     атрибути авторизації.
/// </summary>
public class UserPostController : Controller
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    ///     Конструктор контролера. Ініціалізує доступ до контексту БД.
    /// </summary>
    /// <param name="context">Екземпляр ApplicationDbContext для роботи з даними.</param>
    public UserPostController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Повертає список усіх публікацій користувачів для відображення у вигляді.
    ///     Завантажує автора та теги кожного поста та мапить їх у ViewModel.
    /// </summary>
    /// <returns>HTML-подання зі списком публікацій (Status 200).</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    // GET: UserPost
    public async Task<IActionResult> Index()
    {
        // 1) Витягуємо з БД сутності Post разом із пов’язаними даними.
        // Include(p => p.Author) та Include(p => p.Tags) — явне завантаження навігаційних властивостей,
        // щоб уникнути N+1 запитів і мати все необхідне для побудови ViewModel одразу.
        var applicationDbContext =
            _context.Posts
                .Include(p => p.Author)
                .Include(p => p.Tags);

        // 2) Матеріалізуємо запит асинхронно (ToListAsync), а вже потім мапимо в ViewModel.
        // Чому не проектувати одразу в ViewModel через Select?
        // - Ми централізуємо всю логіку перетворення в PostMapper, дотримуючись SRP.
        // - Це спрощує супровід і повторне використання (один шлях мапінгу Entity -> ViewModel для різних місць).
        // - Зберігаємо «чистоту» мапера (без залежності від EF) і зменшуємо ризик помилок у складних проєкціях LINQ-to-Entities.
        var vm = PostMapper.ToViewModels(await applicationDbContext.ToListAsync());

        // 3) Повертаємо колекцію ViewModel у View — представлення працює зі зручною для UI моделлю,
        // а не з сутністю БД. Це підвищує безпеку (без зайвих полів) і спрощує розмітку.
        return View(vm);
    }

    /// <summary>
    ///     Повертає деталі конкретної публікації за ідентифікатором.
    ///     Завантажує пов’язані дані (автор, теги) і мапить їх у ViewModel для відображення.
    /// </summary>
    /// <param name="id">Ідентифікатор публікації.</param>
    /// <returns>HTML-подання з деталями (200) або 404, якщо запис не знайдено.</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // GET: UserPost/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        // Валідуємо вхідні дані маршруту
        if (id == null) return NotFound();

        // 1) Завантажуємо сутність Post разом із пов’язаними даними, необхідними для відображення.
        //    Include(p => p.Author) та Include(p => p.Tags) — щоб уникнути N+1 та одразу мати повний набір даних для ViewModel.
        var entity = await _context.Posts
            .Include(p => p.Author)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (entity == null) return NotFound();

        // 2) Мапимо Entity -> ViewModel через PostMapper.
        //    Чому не проектуємо напряму в LINQ? Ми централізуємо перетворення у мапері (SRP),
        //    тримаючи його «чистим» (без залежності від DbContext) і забезпечуючи єдиний шлях мапінгу в усьому коді.
        var vm = PostMapper.ToViewModel(entity);

        // 3) Повертаємо ViewModel у представлення — це підвищує безпеку (лише потрібні для UI поля)
        //    та спрощує верстку (готові до відображення значення, форматування дат тощо).
        return View(vm);
    }

    // GET: UserPost/Create
    /// <summary>
    ///     Відображає форму створення нової публікації.
    ///     Доступ лише для автентифікованих користувачів.
    /// </summary>
    /// <returns>HTML-подання форми створення (200) або 401/403 при відсутності доступу.</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize]
    public IActionResult Create()
    {
        ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name");
        return View();
    }

    // POST: UserPost/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    /// <summary>
    ///     Обробляє відправку форми створення публікації: валідаторує дані, створює сутність та зберігає в БД.
    /// </summary>
    /// <param name="data">Дані форми створення публікації.</param>
    /// <returns>
    ///     Редірект на список (302) при успіху або повторне відображення форми з помилками (200). Для неавторизованих —
    ///     401/403.
    /// </returns>
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create([
            Bind("Id,Title,Slug,Content,SelectedTagIds")]
        PostCreateDto data)
    {
        if (ModelState.IsValid)
        {
            // Set the author to the current logged-in user
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError(string.Empty, "Не удалось определить текущего пользователя.");
            }
            else
            {
                var postEntity = PostMapper.ToEntity(data, userId);

                // Пояснення (чому не в Mapper):
                // - Mapper повинен залишатися «чистим» і не знати про джерела даних (DbContext, SQL, HTTP тощо).
                // - Прив’язка тегів потребує доступу до БД (завантаження сутностей Tag за SelectedTagIds),
                //   тому це відповідальність шару доступу до даних або сервісу/контролера, але не мапера.
                // - Так ми дотримуємось SRP (Single Responsibility Principle) і спрощуємо тестування мапера.
                //
                // Альтернатива: винести цей блок у доменний сервіс (наприклад, IPostService.AttachTagsAsync),
                // який інкапсулює роботу з DbContext. Контролер тоді викликатиме сервіс, а мапер залишиться простим.
                if (data.SelectedTagIds != null && data.SelectedTagIds.Count > 0)
                {
                    var tags = await _context.Tags
                        .Where(t => data.SelectedTagIds.Contains(t.Id))
                        .ToListAsync();
                    postEntity.Tags = tags;
                }

                _context.Add(postEntity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }

        // Repopulate select list on validation error with selected values
        ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", data.SelectedTagIds);
        return View(data);
    }

    /// <summary>
    ///     Відображає форму редагування публікації із попереднім заповненням полів і вибраних тегів.
    /// </summary>
    /// <param name="id">Ідентифікатор публікації для редагування.</param>
    /// <returns>HTML-подання форми (200) або 404, якщо запис не знайдено.</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // GET: UserPost/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        // Завантажуємо сутність разом із тегами, щоб попередньо заповнити форму редагування
        var entity = await _context.Posts
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (entity == null) return NotFound();

        // Мапимо сутність -> DTO для редагування. View працює з DTO, а не з Entity (захист від over-posting)
        var dto = PostMapper.ToUpdateDto(entity);

        // Підготуємо список тегів для мультивибору з попередньо вибраними значеннями
        ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", dto.SelectedTagIds);

        return View(dto);
    }

    // POST: UserPost/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    /// <summary>
    ///     Обробляє відправку форми редагування публікації: застосовує зміни та оновлює зв'язки тегів.
    /// </summary>
    /// <param name="id">Ідентифікатор публікації з маршруту.</param>
    /// <param name="data">Дані форми редагування.</param>
    /// <returns>
    ///     Редірект на список (302) при успіху або повторне відображення форми з помилками (200). 404 — якщо запис не
    ///     знайдено. 401/403 — при відсутності доступу.
    /// </returns>
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Slug,Content,SelectedTagIds")] PostUpdateDto data)
    {
        // Перевіряємо узгодженість Id у маршруті та у тілі запиту/форми
        if (id != data.Id) return NotFound();

        if (ModelState.IsValid)
        {
            // 1) Завантажуємо наявну сутність з БД разом із поточними тегами
            var entity = await _context.Posts
                .Include(p => p.Tags)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (entity == null) return NotFound();

            // 2) Застосовуємо зміни через мапер — мапер не звертається до БД (SRP)
            PostMapper.ApplyUpdates(entity, data);

            // 3) Оновлюємо зв'язки тегів (many-to-many) тут, де доступний DbContext, а не в мапері
            var selectedIds = data.SelectedTagIds ?? new List<int>();
            var newTags = await _context.Tags
                .Where(t => selectedIds.Contains(t.Id))
                .ToListAsync();
            entity.Tags = newTags;

            try
            {
                _context.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostModelExists(entity.Id)) return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // Якщо валідація не пройшла — знову підготуємо список тегів із вибраними значеннями та повернемо форму
        ViewData["Tags"] = new MultiSelectList(_context.Tags, "Id", "Name", data.SelectedTagIds);
        return View(data);
    }

    /// <summary>
    ///     Відображає підтвердження видалення публікації.
    /// </summary>
    /// <param name="id">Ідентифікатор публікації для видалення.</param>
    /// <returns>HTML-подання підтвердження (200) або 404, якщо запис не знайдено.</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    // GET: UserPost/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var postModel = await _context.Posts
            .Include(p => p.Author)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (postModel == null) return NotFound();

        return View(postModel);
    }

    // POST: UserPost/Delete/5
    /// <summary>
    ///     Остаточно видаляє публікацію після підтвердження користувачем.
    /// </summary>
    /// <param name="id">Ідентифікатор публікації, яку потрібно видалити.</param>
    /// <returns>Редірект на список публікацій (302). 401/403 — при відсутності доступу.</returns>
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var postModel = await _context.Posts.FindAsync(id);
        if (postModel != null) _context.Posts.Remove(postModel);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    ///     Перевіряє, чи існує публікація з вказаним ідентифікатором.
    /// </summary>
    /// <param name="id">Ідентифікатор публікації.</param>
    /// <returns>True, якщо запис існує; інакше — False.</returns>
    private bool PostModelExists(int id)
    {
        return _context.Posts.Any(e => e.Id == id);
    }
}