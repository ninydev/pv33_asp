using System.Linq.Expressions;
using WebApplication1.Areas.MyTask.Entities;

namespace WebApplication1.Areas.MyTask.Repositories;

/// <summary>
/// Репозиторий доступа к данным для сущности <see cref="TaskEntity"/>.
/// Работает только с сущностями домена. Маппинг в/из DTO выполняется на уровне сервисов/мэпперов.
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// Возвращает запрос для композиции фильтров/сортировки/пагинации на верхних слоях.
    /// По умолчанию без отслеживания, чтобы не держать трекер изменений на списках.
    /// </summary>
    IQueryable<TaskEntity> Query(bool asNoTracking = true);

    /// <summary>
    /// Получить задачу по идентификатору. При необходимости включает навигационные свойства.
    /// </summary>
    Task<TaskEntity?> GetByIdAsync(int id, bool includeAssignee = false, CancellationToken ct = default);

    /// <summary>
    /// Проверка существования задачи по идентификатору.
    /// </summary>
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Создать новую задачу. Возвращает созданную сущность с установленным идентификатором.
    /// </summary>
    Task<TaskEntity> AddAsync(TaskEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Обновить существующую задачу.
    /// </summary>
    Task UpdateAsync(TaskEntity entity, CancellationToken ct = default);

    /// <summary>
    /// Удалить задачу по идентификатору. Возвращает false, если запись не найдена.
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
