using System.ComponentModel.DataAnnotations;
using TaskPriority = WebApplication1.Areas.MyTask.Entities.TaskPriority;
using TaskStatus = WebApplication1.Areas.MyTask.Entities.TaskStatus;

namespace WebApplication1.Areas.MyTask.DTO;

/// <summary>
/// Параметры фильтрации/сортировки/пагинации для списка задач.
/// Суффикс QueryDto — вход из строки запроса.
/// </summary>
public class TaskFilterQueryDto
{
    // Фильтры
    public string? Search { get; set; }
    public TaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public string? AssigneeId { get; set; }
    public DateTimeOffset? DueFrom { get; set; }
    public DateTimeOffset? DueTo { get; set; }

    // Сортировка
    public string? SortBy { get; set; } = "CreatedAt"; // допустимы: CreatedAt, DueDate, Priority, Status, Title
    public bool Desc { get; set; } = true;

    // Пагинация
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 200)]
    public int PageSize { get; set; } = 10;
}
