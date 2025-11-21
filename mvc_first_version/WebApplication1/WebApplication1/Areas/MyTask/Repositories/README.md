Папка: Repositories

Назначение
- Доступ к данным для `TaskEntity`: интерфейсы и реализации (EF Core, Dapper и т. п.).

Рекомендации
- Определяйте интерфейсы (`ITaskRepository`) в этой папке, реализации можно держать рядом (`EfTaskRepository`).
- Репозиторий возвращает сущности и принимает сущности — маппинг в/из DTO выполняется в слое `Mappers`/`Services`.
- Инъекция `DbContext` осуществляется через конструктор реализаций.

Пример интерфейса (эскиз)
```csharp
namespace WebApplication1.Areas.Task.Repositories;

using WebApplication1.Areas.Task.Entities;

public interface ITaskRepository
{
    Task<TaskEntity?> GetAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TaskEntity>> GetListAsync(TaskFilter filter, CancellationToken ct = default);
    Task AddAsync(TaskEntity entity, CancellationToken ct = default);
    Task UpdateAsync(TaskEntity entity, CancellationToken ct = default);
    Task DeleteAsync(TaskEntity entity, CancellationToken ct = default);
}

public record TaskFilter(string? Search, TaskStatus? Status);
```
