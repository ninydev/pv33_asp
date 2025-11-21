using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Areas.Task.Controllers;

[Area("Task")]
public class TaskController : Controller
{
    // Позже внедрим ITaskService через DI

    // GET: /Task/Task/Index
    public IActionResult Index()
    {
        return View();
    }
}
