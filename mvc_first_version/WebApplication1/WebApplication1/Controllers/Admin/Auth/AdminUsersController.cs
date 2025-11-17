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
    
    public ActionResult Index()
    {
        var users = userManager.Users;
        return View("~/Views/Admin/AdminUsers/Index.cshtml", users);
    }
}