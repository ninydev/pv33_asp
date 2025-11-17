using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;
using System.Threading.Tasks;

namespace WebApplication1.Controllers.Admin.Auth;

[Authorize(Roles = "Admin")]
[Route("admin/[controller]")]
public class AdminUsersController : Controller
{
    
    private readonly UserManager<MyIdentityUserEntity> userManager;
    
    public AdminUsersController(UserManager<MyIdentityUserEntity> userManager)
    {
        this.userManager = userManager;
    }
    
    public async Task<ActionResult> Index(int page = 1, int pageSize = 10, 
        string? search = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;

        var query = userManager.Users.AsQueryable();

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
        ViewBag.UserRoles = rolesByUser;
        return View("~/Views/Admin/AdminUsers/Index.cshtml", users);
    }
}