Папка: Controllers (Area: Task)

Назначение
- Контроллеры MVC/Web API, обслуживающие функционал задач (`TaskEntity`).

Правила
- Каждый контроллер помечайте атрибутом `[Area("Task")]`.
- Пространство имён: `WebApplication1.Areas.Task.Controllers`.
- Имена контроллеров оканчивать на `Controller` (например, `TaskController`).
- Не размещайте бизнес‑логику в контроллерах — только валидация входных данных, координация слоёв, формирование ответа.

Пример заготовки
```csharp
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Areas.Task.Controllers;

[Area("Task")]
public class TaskController : Controller
{
    // private readonly ITaskService _taskService;
    // public TaskController(ITaskService taskService) => _taskService = taskService;

    public IActionResult Index() => View();
}
```
