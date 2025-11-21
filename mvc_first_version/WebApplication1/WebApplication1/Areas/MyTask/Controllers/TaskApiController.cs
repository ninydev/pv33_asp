using Microsoft.AspNetCore.Mvc;
using WebApplication1.Areas.MyTask.DTO;
using WebApplication1.Areas.MyTask.Exceptions;
using WebApplication1.Areas.MyTask.Services;

namespace WebApplication1.Areas.MyTask.Controllers;

/// <summary>
/// REST API для управления задачами (TaskEntity).
/// Тонкий контроллер: вся логика в сервисе, контроллер только маршрутизирует и оформляет ответы.
/// </summary>
[ApiController]
[Route("api/mytask/tasks")]
[Produces("application/json")]
public class TaskApiController : ControllerBase
{
    private readonly ITaskService _service;

    public TaskApiController(ITaskService service)
    {
        _service = service;
    }

    /// <summary>
    /// Получить страницу задач с фильтрацией, сортировкой и пагинацией.
    /// </summary>
    /// <param name="query">Параметры фильтра/сортировки/пагинации.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Страничная выдача задач.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<TaskListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<TaskListItemDto>>> GetPage([FromQuery] TaskFilterQueryDto query, CancellationToken ct)
    {
        var page = await _service.GetPageAsync(query, ct);
        return Ok(page);
    }

    /// <summary>
    /// Получить задачу по идентификатору.
    /// </summary>
    [HttpGet("{id:int}", Name = "Task_GetById")]
    [ProducesResponseType(typeof(TaskDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDetailsDto>> GetById(int id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    /// <summary>
    /// Создать новую задачу.
    /// </summary>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(TaskCreatedResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] TaskCreateRequestDto body, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        try
        {
            var created = await _service.CreateAsync(body, id => Url.Link("Task_GetById", new { id }), ct);
            return CreatedAtRoute("Task_GetById", new { id = created.Id }, created);
        }
        catch (Exception ex)
        {
            return MapError(ex);
        }
    }

    /// <summary>
    /// Полностью обновить задачу (PUT).
    /// </summary>
    [HttpPut("{id:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateRequestDto body, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        try
        {
            var ok = await _service.UpdateAsync(id, body, ct);
            if (!ok) return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            return MapError(ex);
        }
    }

    /// <summary>
    /// Частично обновить задачу (PATCH).
    /// </summary>
    [HttpPatch("{id:int}")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Patch(int id, [FromBody] TaskPatchRequestDto body, CancellationToken ct)
    {
        try
        {
            var ok = await _service.PatchAsync(id, body, ct);
            if (!ok) return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            return MapError(ex);
        }
    }

    /// <summary>
    /// Удалить задачу.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        if (!ok) return NotFound();
        return NoContent();
    }

    private ObjectResult MapError(Exception ex)
    {
        var status = MyTaskExceptionMapping.GetHttpStatusCode(ex);
        var payload = MyTaskExceptionMapping.GetErrorInfo(ex);
        return StatusCode(status, payload);
    }
}
