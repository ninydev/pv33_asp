using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers.Admin.Auth;

[Authorize(Roles = "Admin")]
[Route("admin/[controller]")]
public class AdminUsersController : Controller
{
    public ActionResult Index()
    {
        return View();
    }
}