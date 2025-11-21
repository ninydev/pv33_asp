using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Areas.MyTask.Entities;
using WebApplication1.Entities;

namespace WebApplication1.Areas.MyTask.Entities;

/// <summary>
/// Домашняя сущность задачи для хранения в БД (EF Core).
/// Содержит полное состояние задачи.
/// </summary>
public class TaskEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Description { get; set; }

    [Required]
    public TaskStatus Status { get; set; } = TaskStatus.New;

    [Required]
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTimeOffset? DueDate { get; set; }

    [Required]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? UpdatedAt { get; set; }

    // Назначенный исполнитель (опционально)
    public string? AssigneeId { get; set; }
    [ForeignKey(nameof(AssigneeId))]
    public MyIdentityUserEntity? Assignee { get; set; }
}
