using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers.Admin
{
    [Route("admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        // GET: AdminDashboardController
        public ActionResult Index()
        {
            return View();
        }

    }
}
