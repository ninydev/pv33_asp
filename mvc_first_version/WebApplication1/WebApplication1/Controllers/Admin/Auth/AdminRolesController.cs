using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebApplication1.Controllers.Admin.Auth;

[Authorize(Roles = "Admin")]
[Route("admin/[controller]")]
public class AdminRolesController : Controller
{
    
    private readonly RoleManager<IdentityRole> roleManager;
    
    public AdminRolesController(RoleManager<IdentityRole> roleManager)
    {
        this.roleManager = roleManager;
    }
    
    public IActionResult Index()
    {
        var roles = roleManager.Roles;
        return View("~/Views/Admin/AdminRoles/Index.cshtml", roles);
    }
}