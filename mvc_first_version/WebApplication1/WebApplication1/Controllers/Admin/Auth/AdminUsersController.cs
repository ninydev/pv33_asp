using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;

namespace WebApplication1.Controllers.Admin.Auth;

[Authorize(Roles = "Admin")]
[Route("admin/[controller]")]
public class AdminUsersController : Controller
{
    
    private readonly UserManager<MyIdentityUserEntity> userManager;
    private readonly RoleManager<IdentityRole> roleManager;
    
    public AdminUsersController(UserManager<MyIdentityUserEntity> userManager, RoleManager<IdentityRole> roleManager)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
    }
    
    /// <summary>
    /// Список пользователей. Обратите внимание: стандартный логгер получаем 
    /// НЕ через конструктор контроллера, а только для этого метода — через параметр
    /// <c>[FromServices] ILogger&lt;AdminUsersController&gt; logger</c>.
    /// Такой подход:
    /// - Изолирует зависимость логгера только там, где она реально нужна (меньше «шумных» полей класса).
    /// - Упрощает тестирование отдельных экшенов (можно передать заглушку логгера только в этот метод).
    /// - Чуть менее производителен микроскопически из‑за получения зависимости на каждый вызов экшена,
    ///   но в ASP.NET Core это обычно пренебрежимо.
    /// В отличие от инъекции через конструктор контроллера, где сервис доступен во всех методах как поле класса,
    /// методная инъекция делает зависимость локальной и явно видимой в сигнатуре конкретного экшена.
    /// </summary>
    [HttpGet("", Name = "AdminUsers_Index")]
    public async Task<ActionResult> Index([FromServices] ILogger<AdminUsersController> logger, int page = 1, int pageSize = 10, 
        string? search = null, string? role = null)
    {
        logger.LogInformation("AdminUsers.Index called: page={Page}, pageSize={PageSize}, search={Search}, role={Role}", page, pageSize, search, role);
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;

        var query = userManager.Users.AsQueryable();

        // Фильтр по роли (если указана)
        if (!string.IsNullOrWhiteSpace(role))
        {
            var usersInRole = await userManager.GetUsersInRoleAsync(role);
            var ids = usersInRole.Select(u => u.Id).ToList();
            query = query.Where(u => ids.Contains(u.Id));
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(u => u.Email!.Contains(term));
        }

        query = query.OrderBy(u => u.Email);

        var totalItems = query.Count();
        var users = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Собираем роли только для пользователей на текущей странице
        var rolesByUser = new Dictionary<string, IList<string>>();
        foreach (var u in users)
        {
            var roles = await userManager.GetRolesAsync(u);
            rolesByUser[u.Id] = roles;
        }

        ViewData["Page"] = page;
        ViewData["PageSize"] = pageSize;
        ViewData["TotalItems"] = totalItems;
        ViewData["Search"] = search ?? string.Empty;
        ViewData["Role"] = role ?? string.Empty;
        ViewBag.AllRoles = roleManager.Roles.Select(r => r.Name!).OrderBy(n => n).ToList();
        ViewBag.UserRoles = rolesByUser;
        logger.LogInformation("AdminUsers.Index prepared response: usersOnPage={UsersOnPage}, totalItems={TotalItems}", users.Count, totalItems);
        return View("~/Views/Admin/AdminUsers/Index.cshtml", users);
    }

    [HttpGet("edit/{id}", Name = "AdminUsers_Edit")]
    public async Task<IActionResult> Edit(string id, int page = 1, int pageSize = 10, string? search = null, string? role = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            TempData["StatusMessage"] = "Не указан пользователь";
            return RedirectToRoute("AdminUsers_Index", new { page, pageSize, search, role });
        }

        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["StatusMessage"] = "Пользователь не найден";
            return RedirectToRoute("AdminUsers_Index", new { page, pageSize, search, role });
        }

        var allRoles = roleManager.Roles.Select(r => r.Name!).OrderBy(n => n).ToList();
        var userRoles = await userManager.GetRolesAsync(user);

        ViewBag.AllRoles = allRoles;
        ViewBag.UserRoles = userRoles; // список ролей конкретного пользователя

        ViewData["Page"] = page;
        ViewData["PageSize"] = pageSize;
        ViewData["Search"] = search ?? string.Empty;
        ViewData["Role"] = role ?? string.Empty;

        return View("~/Views/Admin/AdminUsers/Edit.cshtml", user);
    }

    [HttpPost("edit/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSave(string id, string? selectedRole, int page = 1, int pageSize = 10, string? search = null, string? role = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            TempData["StatusMessage"] = "Не указан пользователь";
            return RedirectToRoute("AdminUsers_Index", new { page, pageSize, search, role });
        }

        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["StatusMessage"] = "Пользователь не найден";
            return RedirectToRoute("AdminUsers_Index", new { page, pageSize, search, role });
        }

        // Удаляем все текущие роли
        var currentRoles = await userManager.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            var removeRes = await userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeRes.Succeeded)
            {
                TempData["StatusMessage"] = "Не удалось снять текущие роли пользователя.";
                return RedirectToRoute("AdminUsers_Index", new { page, pageSize, search, role });
            }
        }

        // Назначаем выбранную роль, если она указана
        if (!string.IsNullOrWhiteSpace(selectedRole))
        {
            if (!await roleManager.RoleExistsAsync(selectedRole))
            {
                TempData["StatusMessage"] = $"Роль '{selectedRole}' не существует.";
                return RedirectToRoute("AdminUsers_Index", new { page, pageSize, search, role });
            }

            var addRes = await userManager.AddToRoleAsync(user, selectedRole);
            if (!addRes.Succeeded)
            {
                TempData["StatusMessage"] = "Не удалось назначить роль пользователю.";
                return RedirectToRoute("AdminUsers_Index", new { page, pageSize, search, role });
            }
        }

        TempData["StatusMessage"] = "Изменения сохранены.";
        return RedirectToRoute("AdminUsers_Index", new { page, pageSize, search, role });
    }

    [HttpPost("lock")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Lock(string id, int page = 1, int pageSize = 10, string? search = null, string? role = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            TempData["StatusMessage"] = "Не указан пользователь";
            return RedirectToRoute("AdminUsers_Index", new { page, pageSize, search, role });
        }

        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["StatusMessage"] = "Пользователь не найден";
            return RedirectToRoute("AdminUsers_Index", new { page, pageSize, search, role });
        }

        // Включаем возможность блокировки и ставим большую дату блокировки
        await userManager.SetLockoutEnabledAsync(user, true);
        var result = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

        TempData["StatusMessage"] = result.Succeeded
            ? $"Пользователь {user.Email ?? user.UserName} заблокирован."
            : "Не удалось заблокировать пользователя.";

        return RedirectToRoute("AdminUsers_Index", new { page, pageSize, search, role });
    }

    [HttpPost("unlock")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unlock(string id, int page = 1, int pageSize = 10, string? search = null, string? role = null)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            TempData["StatusMessage"] = "Не указан пользователь";
            return RedirectToRoute("AdminUsers_Index", new { page, pageSize, search, role });
        }

        var user = await userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["StatusMessage"] = "Пользователь не найден";
            return RedirectToRoute("AdminUsers_Index", new { page, pageSize, search, role });
        }

        var r1 = await userManager.SetLockoutEndDateAsync(user, null);
        var r2 = await userManager.ResetAccessFailedCountAsync(user);
        var succeeded = r1.Succeeded && r2.Succeeded;

        TempData["StatusMessage"] = succeeded
            ? $"Пользователь {user.Email ?? user.UserName} разблокирован."
            : "Не удалось разблокировать пользователя.";

        return RedirectToRoute("AdminUsers_Index", new { page, pageSize, search, role });
    }
}