namespace WebApplication1.Areas.MyTask.Exceptions;

/// <summary>
/// Исключение прикладной валидации. Используйте для бизнес-правил и проверок модели,
/// отличных от технической ModelState-валидации MVC.
/// </summary>
public class TaskValidationException : MyTaskException
{
    public TaskValidationException(string message)
        : base(message, MyTaskErrorCode.ValidationFailed)
    {
    }

    public TaskValidationException(string field, string message)
        : base("Ошибки валидации.", MyTaskErrorCode.ValidationFailed,
            new Dictionary<string, string[]> { { field, new[] { message } } })
    {
    }

    public TaskValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("Ошибки валидации.", MyTaskErrorCode.ValidationFailed, errors)
    {
    }

    public TaskValidationException(string message, Exception? inner)
        : base(message, inner, MyTaskErrorCode.ValidationFailed)
    {
    }
}
