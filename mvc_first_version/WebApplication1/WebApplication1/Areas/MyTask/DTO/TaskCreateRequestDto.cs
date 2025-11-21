using System.ComponentModel.DataAnnotations;
using TaskPriority = WebApplication1.Areas.MyTask.Entities.TaskPriority;

namespace WebApplication1.Areas.MyTask.DTO;

/// <summary>
/// Входная модель для создания задачи.
/// Суффикс RequestDto — вход от клиента.
/// </summary>
public class TaskCreateRequestDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Description { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTimeOffset? DueDate { get; set; }

    /// <summary>
    /// Идентификатор назначаемого пользователя (опционально).
    /// </summary>
    public string? AssigneeId { get; set; }
}
