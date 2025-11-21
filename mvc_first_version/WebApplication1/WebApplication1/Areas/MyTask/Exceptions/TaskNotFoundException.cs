namespace WebApplication1.Areas.MyTask.Exceptions;

/// <summary>
/// Исключение «задача не найдена».
/// Бросайте при обращении к несуществующему идентификатору.
/// </summary>
public class TaskNotFoundException : MyTaskException
{
    public int? TaskId { get; }

    public TaskNotFoundException(int id)
        : base($"Задача с Id={id} не найдена.", MyTaskErrorCode.NotFound)
    {
        TaskId = id;
    }

    public TaskNotFoundException(string message)
        : base(message, MyTaskErrorCode.NotFound)
    {
    }

    public TaskNotFoundException(string message, Exception? inner)
        : base(message, inner, MyTaskErrorCode.NotFound)
    {
    }
}
