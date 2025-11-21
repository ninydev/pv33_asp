using System.Text.Json.Serialization;

namespace WebApplication1.Areas.MyTask.Entities;

/// <summary>
/// Приоритет задачи.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TaskPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3
}
