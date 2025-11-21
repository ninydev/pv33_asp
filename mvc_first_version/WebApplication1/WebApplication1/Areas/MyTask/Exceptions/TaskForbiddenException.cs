namespace WebApplication1.Areas.MyTask.Exceptions;

/// <summary>
/// Ошибка авторизации/прав доступа на уровне домена MyTask.
/// </summary>
public class TaskForbiddenException : MyTaskException
{
    public string? Action { get; }
    public string? UserId { get; }

    public TaskForbiddenException(string message, string? action = null, string? userId = null)
        : base(message, MyTaskErrorCode.Forbidden)
    {
        Action = action;
        UserId = userId;
    }

    public TaskForbiddenException(string message, Exception? inner, string? action = null, string? userId = null)
        : base(message, inner, MyTaskErrorCode.Forbidden)
    {
        Action = action;
        UserId = userId;
    }
}
