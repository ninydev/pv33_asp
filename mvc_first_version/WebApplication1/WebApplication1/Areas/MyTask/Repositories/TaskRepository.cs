using Microsoft.EntityFrameworkCore;
using WebApplication1.Areas.MyTask.Entities;
using WebApplication1.Data;

namespace WebApplication1.Areas.MyTask.Repositories;

/// <summary>
/// Реализация репозитория для <see cref="TaskEntity"/> на базе EF Core.
/// </summary>
public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _db;

    public TaskRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public IQueryable<TaskEntity> Query(bool asNoTracking = true)
    {
        var q = _db.Set<TaskEntity>().AsQueryable();
        return asNoTracking ? q.AsNoTracking() : q;
    }

    /// <inheritdoc />
    public async Task<TaskEntity?> GetByIdAsync(int id, bool includeAssignee = false, CancellationToken ct = default)
    {
        IQueryable<TaskEntity> q = _db.Set<TaskEntity>();
        if (includeAssignee)
        {
            q = q.Include(t => t.Assignee);
        }

        return await q.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
    {
        return _db.Set<TaskEntity>().AnyAsync(t => t.Id == id, ct);
    }

    /// <inheritdoc />
    public async Task<TaskEntity> AddAsync(TaskEntity entity, CancellationToken ct = default)
    {
        // Проставляем системные поля
        if (entity.CreatedAt == default)
        {
            entity.CreatedAt = DateTimeOffset.UtcNow;
        }
        entity.UpdatedAt = null; // только при создании

        await _db.Set<TaskEntity>().AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
        return entity;
    }

    /// <inheritdoc />
    public async Task UpdateAsync(TaskEntity entity, CancellationToken ct = default)
    {
        // Обновляем системные поля
        entity.UpdatedAt = DateTimeOffset.UtcNow;

        // Если сущность не отслеживается, прикрепляем и помечаем измененной
        var entry = _db.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            _db.Attach(entity);
            entry = _db.Entry(entity);
        }
        entry.State = EntityState.Modified;

        await _db.SaveChangesAsync(ct);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Set<TaskEntity>().FindAsync(new object?[] { id }, ct);
        if (entity == null)
            return false;

        _db.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
