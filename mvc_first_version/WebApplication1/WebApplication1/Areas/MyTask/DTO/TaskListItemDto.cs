using WebApplication1.Areas.MyTask.Entities;
using TaskStatus = WebApplication1.Areas.MyTask.Entities.TaskStatus;
using TaskPriority = WebApplication1.Areas.MyTask.Entities.TaskPriority;

namespace WebApplication1.Areas.MyTask.DTO;

/// <summary>
/// Краткая карточка задачи для списков.
/// Суффикс Dto — объект передачи данных наружу.
/// </summary>
public class TaskListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public string? AssigneeId { get; set; }
    public string? AssigneeUserName { get; set; }
}
