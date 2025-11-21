using TaskPriority = WebApplication1.Areas.MyTask.Entities.TaskPriority;
using TaskStatus = WebApplication1.Areas.MyTask.Entities.TaskStatus;

namespace WebApplication1.Areas.MyTask.DTO;

/// <summary>
/// Полная модель задачи для детального просмотра.
/// </summary>
public class TaskDetailsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public string? AssigneeId { get; set; }
    public string? AssigneeUserName { get; set; }
    public string? AssigneeEmail { get; set; }
}
