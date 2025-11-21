using Microsoft.AspNetCore.Mvc;
using WebApplication1.Areas.MyTask.DTO;
using WebApplication1.Areas.MyTask.Services;

namespace WebApplication1.Areas.MyTask.Controllers;

[Area("MyTask")]
public class TaskController : Controller
{
    private readonly ITaskService _service;

    public TaskController(ITaskService service)
    {
        _service = service;
    }

    // GET: /MyTask/Task/Index
    public async Task<IActionResult> Index([FromQuery] TaskFilterQueryDto query, CancellationToken ct)
        => View(await _service.GetPageAsync(query, ct));

    // GET: /MyTask/Task/Details/5
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();
        return View(dto);
    }

    // GET: /MyTask/Task/Create
    public IActionResult Create()
        => View(new TaskCreateRequestDto());

    // POST: /MyTask/Task/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskCreateRequestDto model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(model);

        var created = await _service.CreateAsync(model, id => Url.Action("Details", new { id, area = "MyTask" }), ct);
        return RedirectToAction(nameof(Details), new { id = created.Id, area = "MyTask" });
    }

    // GET: /MyTask/Task/Edit/5
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var details = await _service.GetByIdAsync(id, ct);
        if (details == null) return NotFound();

        var model = new TaskUpdateRequestDto
        {
            Title = details.Title,
            Description = details.Description,
            Status = details.Status,
            Priority = details.Priority,
            DueDate = details.DueDate,
            AssigneeId = details.AssigneeId
        };
        ViewBag.TaskId = id;
        return View(model);
    }

    // POST: /MyTask/Task/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TaskUpdateRequestDto model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.TaskId = id;
            return View(model);
        }

        var ok = await _service.UpdateAsync(id, model, ct);
        if (!ok) return NotFound();
        return RedirectToAction(nameof(Details), new { id, area = "MyTask" });
    }

    // POST: /MyTask/Task/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        if (!ok) return NotFound();
        return RedirectToAction(nameof(Index));
    }
}
