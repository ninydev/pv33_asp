Папка: Services

Назначение
- Прикладная логика и сценарии использования для работы с задачами (`TaskEntity`).

Рекомендации
- Определяйте интерфейсы (`ITaskService`) и их реализации (`TaskService`).
- Сервисы не зависят от UI и БД; обращаются к репозиториям через интерфейсы.
- Валидация входных DTO, управление транзакциями (при необходимости), публикация доменных событий.

Пример интерфейса
```csharp
namespace WebApplication1.Areas.Task.Services;

public interface ITaskService
{
    Task<int> CreateAsync(TaskCreateDto dto, CancellationToken ct = default);
    Task UpdateAsync(int id, TaskUpdateDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<TaskDetailsDto?> GetAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TaskItemDto>> GetListAsync(TaskFilterDto filter, CancellationToken ct = default);
}
```
