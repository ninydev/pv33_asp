using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskPriority = WebApplication1.Areas.MyTask.Entities.TaskPriority;
using TaskStatus = WebApplication1.Areas.MyTask.Entities.TaskStatus;

namespace WebApplication1.Areas.MyTask.DTO;

/// <summary>
/// Входная модель для частичного обновления задачи (PATCH).
/// Все поля — опциональны. Передавайте только то, что нужно изменить.
/// </summary>
public class TaskPatchRequestDto
{
    [MaxLength(200)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Title { get; set; }

    [MaxLength(4000)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TaskStatus? Status { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public TaskPriority? Priority { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? DueDate { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AssigneeId { get; set; }
}
