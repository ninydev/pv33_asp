Папка: Exceptions

Назначение
- Прикладные (доменные) исключения для области задач. Используются в `Services` и при необходимости обрабатываются в middleware/фильтрах.

Рекомендации
- Создавайте специальные типы вместо использования общих `Exception`/`InvalidOperationException`.
- Исключения должны быть информативны и не содержать бизнес‑логики.

Примеры (эскизы)
```csharp
namespace WebApplication1.Areas.Task.Exceptions;

public class TaskNotFoundException : Exception
{
    public TaskNotFoundException(int id)
        : base($"Task with id={id} not found") {}
}

public class TaskValidationException : Exception
{
    public TaskValidationException(string message) : base(message) {}
}
```
