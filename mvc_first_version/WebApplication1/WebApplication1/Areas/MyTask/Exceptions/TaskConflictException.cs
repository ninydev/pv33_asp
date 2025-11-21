namespace WebApplication1.Areas.MyTask.Exceptions;

/// <summary>
/// Бизнес-конфликт, например, попытка создать дубликат или нарушить инвариант.
/// </summary>
public class TaskConflictException : MyTaskException
{
    public string? Resource { get; }
    public string? Key { get; }

    public TaskConflictException(string message, string? resource = null, string? key = null)
        : base(message, MyTaskErrorCode.Conflict)
    {
        Resource = resource;
        Key = key;
    }

    public TaskConflictException(string message, Exception? inner, string? resource = null, string? key = null)
        : base(message, inner, MyTaskErrorCode.Conflict)
    {
        Resource = resource;
        Key = key;
    }
}
