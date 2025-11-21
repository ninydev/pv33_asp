using System.Text.Json.Serialization;

namespace WebApplication1.Areas.MyTask.Entities;

/// <summary>
/// Статус задачи в жизненном цикле.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskStatus
{
    New = 0,
    InProgress = 1,
    Completed = 2,
    OnHold = 3,
    Cancelled = 4
}
