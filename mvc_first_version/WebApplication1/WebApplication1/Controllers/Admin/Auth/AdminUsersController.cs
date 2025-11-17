using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;

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
    
    public ActionResult Index(int page = 1, int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;

        var query = userManager.Users.OrderBy(u => u.Email);
        var totalItems = query.Count();
        var users = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewData["Page"] = page;
        ViewData["PageSize"] = pageSize;
        ViewData["TotalItems"] = totalItems;
        return View("~/Views/Admin/AdminUsers/Index.cshtml", users);
    }
}