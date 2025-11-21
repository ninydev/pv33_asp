Папка: Entities

Назначение
- Доменные модели (сущности) области задач, например `TaskEntity`.

Рекомендации
- Сущности должны быть «тонкими»: только данные и инварианты, без инфраструктурной логики.
- Атрибуты EF Core допустимы при использовании Code First.
- Избегайте циклических зависимостей между сущностями.

Пример структуры сущности (эскиз)
```csharp
namespace WebApplication1.Areas.Task.Entities;

public class TaskEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.New;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; }
}

public enum TaskStatus { New, InProgress, Done, Archived }
```
