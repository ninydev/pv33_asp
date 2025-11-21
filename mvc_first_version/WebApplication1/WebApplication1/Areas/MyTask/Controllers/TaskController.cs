using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Areas.MyTask.Controllers;

[Area("MyTask")]
public class TaskController : Controller
{
    // Позже внедрим ITaskService через DI

    // GET: /MyTask/Task/Index
    public IActionResult Index()
    {
        return View();
    }
}
