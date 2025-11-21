using WebApplication1.Areas.MyTask.DTO;

namespace WebApplication1.Areas.MyTask.Services;

/// <summary>
/// Прикладной сервис для работы с задачами. Оперирует DTO-моделями, инкапсулируя
/// доступ к данным через репозиторий и маппинг между Entity и DTO.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Получить страницу задач с фильтрацией/сортировкой.
    /// </summary>
    Task<PagedResultDto<TaskListItemDto>> GetPageAsync(TaskFilterQueryDto query, CancellationToken ct = default);

    /// <summary>
    /// Получить детальную информацию по задаче.
    /// </summary>
    Task<TaskDetailsDto?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Создать новую задачу и вернуть идентификатор и опциональный Location.
    /// </summary>
    Task<TaskCreatedResponseDto> CreateAsync(TaskCreateRequestDto dto, Func<int, string?>? buildLocation = null, CancellationToken ct = default);

    /// <summary>
    /// Полное обновление (PUT). Возвращает false, если запись не найдена.
    /// </summary>
    Task<bool> UpdateAsync(int id, TaskUpdateRequestDto dto, CancellationToken ct = default);

    /// <summary>
    /// Частичное обновление (PATCH). Возвращает false, если запись не найдена.
    /// </summary>
    Task<bool> PatchAsync(int id, TaskPatchRequestDto dto, CancellationToken ct = default);

    /// <summary>
    /// Удаление. Возвращает false, если запись не найдена.
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
