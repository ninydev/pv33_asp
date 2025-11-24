using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.MyTask.DTO;
using WebApplication1.Areas.MyTask.Entities;
using WebApplication1.Areas.MyTask.Repositories;
using TaskStatus = WebApplication1.Areas.MyTask.Entities.TaskStatus;

#if AUTO_MAPPER
using AutoMapper;
using AutoMapper.QueryableExtensions;
#endif

namespace WebApplication1.Areas.MyTask.Services;

/// <summary>
/// Реализация прикладного сервиса для работы с задачами.
/// Инкапсулирует доступ к данным через репозиторий и выполняет маппинг между Entity и DTO.
/// </summary>
public class TaskService : ITaskService
{
    private readonly ITaskRepository _repo;
#if AUTO_MAPPER
    private readonly IMapper _mapper;
#endif

    public TaskService(
        ITaskRepository repo
#if AUTO_MAPPER
        , IMapper mapper
#endif
    )
    {
        _repo = repo;
#if AUTO_MAPPER
        _mapper = mapper;
#endif
    }

    /// <inheritdoc />
    public async Task<PagedResultDto<TaskListItemDto>> GetPageAsync(TaskFilterQueryDto q, CancellationToken ct = default)
    {
        var query = _repo.Query(asNoTracking: true);

        // Фильтры
        if (!string.IsNullOrWhiteSpace(q.Search))
            query = query.Where(t => t.Title.Contains(q.Search));
        if (q.Status.HasValue)
            query = query.Where(t => t.Status == q.Status);
        if (q.Priority.HasValue)
            query = query.Where(t => t.Priority == q.Priority);
        if (!string.IsNullOrEmpty(q.AssigneeId))
            query = query.Where(t => t.AssigneeId == q.AssigneeId);
        if (q.DueFrom.HasValue)
            query = query.Where(t => t.DueDate >= q.DueFrom);
        if (q.DueTo.HasValue)
            query = query.Where(t => t.DueDate <= q.DueTo);

        // ВАЖНО: SQLite не поддерживает ORDER BY по DateTimeOffset. Поэтому:
        // 1) сначала считаем total БЕЗ сортировки;
        // 2) сортировку по полям-девтаймам (DueDate/CreatedAt) выполняем на клиенте.

        var total = await query.CountAsync(ct);

        // Сортировка на стороне БД для безопасных типов (не DateTimeOffset)
        var orderAppliedOnServer = false;
        if (!string.IsNullOrWhiteSpace(q.SortBy))
        {
            switch (q.SortBy)
            {
                // DueDate потенциально DateTimeOffset — сортируем на клиенте (см. ниже)
                case "Priority":
                    query = q.Desc ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority);
                    orderAppliedOnServer = true;
                    break;
                case "Status":
                    query = q.Desc ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status);
                    orderAppliedOnServer = true;
                    break;
                case "Title":
                    query = q.Desc ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title);
                    orderAppliedOnServer = true;
                    break;
            }
        }
        // Значение по умолчанию — безопасная сортировка по Id (вместо CreatedAt)
        if (!orderAppliedOnServer && (string.IsNullOrWhiteSpace(q.SortBy) || q.SortBy is not ("Priority" or "Status" or "Title")))
        {
            query = query.OrderByDescending(t => t.Id);
            orderAppliedOnServer = true;
        }

        // Если запрос просит сортировку по DueDate — делаем сортировку и пагинацию на клиенте,
        // чтобы избежать ошибки SQLite "ORDER BY DateTimeOffset not supported".
        var sortByDueDate = string.Equals(q.SortBy, "DueDate", StringComparison.OrdinalIgnoreCase);

#if AUTO_MAPPER
        List<TaskListItemDto> items;
        if (sortByDueDate)
        {
            items = await query
                .ProjectTo<TaskListItemDto>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            items = (q.Desc
                    ? items.OrderByDescending(i => i.DueDate)
                    : items.OrderBy(i => i.DueDate))
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .ToList();
        }
        else
        {
            items = await query
                .ProjectTo<TaskListItemDto>(_mapper.ConfigurationProvider)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .ToListAsync(ct);
        }
#else
        List<TaskListItemDto> items;
        if (sortByDueDate)
        {
            items = await query
                .Select(t => new TaskListItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    Priority = t.Priority,
                    DueDate = t.DueDate,
                    AssigneeId = t.AssigneeId,
                    AssigneeUserName = t.Assignee != null ? t.Assignee.UserName : null
                })
                .ToListAsync(ct);

            items = (q.Desc
                    ? items.OrderByDescending(i => i.DueDate)
                    : items.OrderBy(i => i.DueDate))
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .ToList();
        }
        else
        {
            items = await query
                .Select(t => new TaskListItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    Priority = t.Priority,
                    DueDate = t.DueDate,
                    AssigneeId = t.AssigneeId,
                    AssigneeUserName = t.Assignee != null ? t.Assignee.UserName : null
                })
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .ToListAsync(ct);
        }
#endif

        return new PagedResultDto<TaskListItemDto>
        {
            Items = items,
            TotalCount = total,
            Page = q.Page,
            PageSize = q.PageSize
        };
    }

    /// <inheritdoc />
    public async Task<TaskDetailsDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, includeAssignee: true, ct);
        if (entity == null) return null;

#if AUTO_MAPPER
        return _mapper.Map<TaskDetailsDto>(entity);
#else
        return MapToDetailsDto(entity);
#endif
    }

    /// <inheritdoc />
    public async Task<TaskCreatedResponseDto> CreateAsync(TaskCreateRequestDto dto, Func<int, string?>? buildLocation = null, CancellationToken ct = default)
    {
#if AUTO_MAPPER
        var entity = _mapper.Map<TaskEntity>(dto);
#else
        var entity = MapFromCreateDto(dto);
#endif
        var created = await _repo.AddAsync(entity, ct);

        return new TaskCreatedResponseDto
        {
            Id = created.Id,
            Location = buildLocation?.Invoke(created.Id)
        };
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(int id, TaskUpdateRequestDto dto, CancellationToken ct = default)
    {
        // Проверяем существование, чтобы корректно вернуть false при NotFound
        if (!await _repo.ExistsAsync(id, ct))
            return false;

#if AUTO_MAPPER
        var entity = _mapper.Map<TaskEntity>(dto);
#else
        var entity = new TaskEntity();
        ApplyUpdateDto(entity, dto);
#endif
        entity.Id = id;

        await _repo.UpdateAsync(entity, ct);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> PatchAsync(int id, TaskPatchRequestDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, includeAssignee: false, ct);
        if (entity == null) return false;

#if AUTO_MAPPER
        // Для PATCH профиль настроен маппить только непустые поля
        _mapper.Map(dto, entity);
#else
        ApplyPatchDto(entity, dto);
#endif

        await _repo.UpdateAsync(entity, ct);
        return true;
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => _repo.DeleteAsync(id, ct);

#region Manual mapping helpers (используются, если не подключён AutoMapper)
#if !AUTO_MAPPER
    private static TaskDetailsDto MapToDetailsDto(TaskEntity e)
        => new TaskDetailsDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            Status = e.Status,
            Priority = e.Priority,
            DueDate = e.DueDate,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            AssigneeId = e.AssigneeId,
            AssigneeUserName = e.Assignee?.UserName,
            AssigneeEmail = e.Assignee?.Email
        };

    private static TaskEntity MapFromCreateDto(TaskCreateRequestDto d)
        => new TaskEntity
        {
            Title = d.Title,
            Description = d.Description,
            Priority = d.Priority,
            // Статус по умолчанию — New
            Status = TaskStatus.New,
            DueDate = d.DueDate,
            AssigneeId = d.AssigneeId
        };

    private static void ApplyUpdateDto(TaskEntity e, TaskUpdateRequestDto d)
    {
        e.Title = d.Title;
        e.Description = d.Description;
        e.Status = d.Status;
        e.Priority = d.Priority;
        e.DueDate = d.DueDate;
        e.AssigneeId = d.AssigneeId;
    }

    private static void ApplyPatchDto(TaskEntity e, TaskPatchRequestDto d)
    {
        if (d.Title != null) e.Title = d.Title;
        if (d.Description != null) e.Description = d.Description;
        if (d.Status.HasValue) e.Status = d.Status.Value;
        if (d.Priority.HasValue) e.Priority = d.Priority.Value;
        if (d.DueDate.HasValue) e.DueDate = d.DueDate;
        if (d.AssigneeId != null) e.AssigneeId = d.AssigneeId;
    }
#endif
#endregion
}
