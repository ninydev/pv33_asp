using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebApplication1.Controllers.Admin.Auth;

[Authorize(Roles = "Admin")]
[Route("admin/[controller]")]
public class AdminRolesController : Controller
{
    public IActionResult Index()
    {
        return View("~/Views/Admin/AdminRoles/Index.cshtml");
    }
}