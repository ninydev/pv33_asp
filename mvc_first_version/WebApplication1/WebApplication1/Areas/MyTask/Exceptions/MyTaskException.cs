using System.Diagnostics.CodeAnalysis;

namespace WebApplication1.Areas.MyTask.Exceptions;

/// <summary>
/// База для прикладных исключений области MyTask.
/// Содержит строго типизированный код ошибки и (опционально) набор ошибок валидации.
/// </summary>
public class MyTaskException : Exception
{
    /// <summary>
    /// Код ошибки домена.
    /// </summary>
    public MyTaskErrorCode ErrorCode { get; }

    /// <summary>
    /// Детали валидации (опционально). Ключ — имя поля/области, значение — список сообщений.
    /// </summary>
    public IReadOnlyDictionary<string, string[]>? Errors { get; }

    public MyTaskException(
        string message,
        MyTaskErrorCode errorCode = MyTaskErrorCode.Unknown,
        IReadOnlyDictionary<string, string[]>? errors = null)
        : base(message)
    {
        ErrorCode = errorCode;
        Errors = errors;
    }

    public MyTaskException(
        string message,
        Exception? innerException,
        MyTaskErrorCode errorCode = MyTaskErrorCode.Unknown,
        IReadOnlyDictionary<string, string[]>? errors = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Errors = errors;
    }
}
