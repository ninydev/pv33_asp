using System.ComponentModel.DataAnnotations;
using TaskPriority = WebApplication1.Areas.MyTask.Entities.TaskPriority;
using TaskStatus = WebApplication1.Areas.MyTask.Entities.TaskStatus;

namespace WebApplication1.Areas.MyTask.DTO;

/// <summary>
/// Входная модель для полного обновления задачи (PUT).
/// </summary>
public class TaskUpdateRequestDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Description { get; set; }

    [Required]
    public TaskStatus Status { get; set; }

    [Required]
    public TaskPriority Priority { get; set; }

    public DateTimeOffset? DueDate { get; set; }

    public string? AssigneeId { get; set; }
}
