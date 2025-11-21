namespace WebApplication1.Areas.MyTask.Exceptions;

/// <summary>
/// Коды ошибок домена MyTask для стабильной идентификации причин исключений.
/// </summary>
public enum MyTaskErrorCode
{
    Unknown = 0,

    // Базовые сценарии
    NotFound = 10,
    ValidationFailed = 11,
    Conflict = 12,
    Forbidden = 13,
    Concurrency = 14
}
