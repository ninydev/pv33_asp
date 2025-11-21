namespace WebApplication1.Areas.MyTask.Exceptions;

/// <summary>
/// Ошибка конкурентного обновления (optimistic concurrency).
/// Используйте, когда запись была изменена/удалена параллельно и операция не может быть завершена.
/// </summary>
public class TaskConcurrencyException : MyTaskException
{
    public int? TaskId { get; }

    public TaskConcurrencyException(string message, int? taskId = null)
        : base(message, MyTaskErrorCode.Concurrency)
    {
        TaskId = taskId;
    }

    public TaskConcurrencyException(string message, Exception? inner, int? taskId = null)
        : base(message, inner, MyTaskErrorCode.Concurrency)
    {
        TaskId = taskId;
    }
}
