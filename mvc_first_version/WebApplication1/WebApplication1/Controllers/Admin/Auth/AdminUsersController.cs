using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;
using System.Threading.Tasks;
using System.Linq;
using System;

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
    
    [HttpGet("", Name = "AdminUsers_Index")]
    public async Task<ActionResult> Index(int page = 1, int pageSize = 10, 
        string? search = null, string? role = null)
    {
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
        return View("~/Views/Admin/AdminUsers/Index.cshtml", users);
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