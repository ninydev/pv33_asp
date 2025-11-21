namespace WebApplication1.Areas.MyTask.Exceptions;

/// <summary>
/// Вспомогательные методы для преобразования исключений домена MyTask
/// в HTTP-коды и компактные payload-объекты, пригодные для ProblemDetails.
/// Не требует ASP.NET зависимостей.
/// </summary>
public static class MyTaskExceptionMapping
{
    /// <summary>
    /// Определяет рекомендуемый HTTP-статус по типу исключения.
    /// </summary>
    public static int GetHttpStatusCode(Exception ex)
        => ex switch
        {
            TaskNotFoundException => 404,
            TaskValidationException => 400,
            TaskConflictException => 409,
            TaskForbiddenException => 403,
            TaskConcurrencyException => 409,
            MyTaskException => 400, // прочие прикладные — Bad Request по умолчанию
            _ => 500
        };

    /// <summary>
    /// Минимальная структура ошибки, совместимая с ProblemDetails-подобной моделью.
    /// </summary>
    public sealed record ErrorInfo(
        string Title,
        string Detail,
        string Code,
        IReadOnlyDictionary<string, string[]>? Errors
    );

    /// <summary>
    /// Формирует полезную нагрузку для ответа об ошибке.
    /// </summary>
    public static ErrorInfo GetErrorInfo(Exception ex)
    {
        if (ex is MyTaskException mex)
        {
            return new ErrorInfo(
                Title: GetTitle(mex),
                Detail: mex.Message,
                Code: mex.ErrorCode.ToString(),
                Errors: mex.Errors
            );
        }

        return new ErrorInfo(
            Title: "Внутренняя ошибка",
            Detail: ex.Message,
            Code: MyTaskErrorCode.Unknown.ToString(),
            Errors: null
        );
    }

    private static string GetTitle(MyTaskException ex) => ex switch
    {
        TaskNotFoundException => "Ресурс не найден",
        TaskValidationException => "Ошибки валидации",
        TaskConflictException => "Конфликт запроса",
        TaskForbiddenException => "Доступ запрещён",
        TaskConcurrencyException => "Конкурентное обновление",
        _ => "Ошибка запроса"
    };
}
