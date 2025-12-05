using System.Linq.Expressions;
using LiveBlog.Data;
using LiveBlog.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace LiveBlog.Repositories.Base;

/// <summary>
/// Базова реалізація репозиторію на EF Core для узагальнених CRUD-операцій.
/// </summary>
/// <typeparam name="TEntity">Тип сутності.</typeparam>
public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly ApplicationDbContext Db;
    protected readonly DbSet<TEntity> Set;

    public EfRepository(ApplicationDbContext db)
    {
        Db = db;
        Set = db.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Set.FindAsync(new object?[] { id }, cancellationToken: cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = Set;
        if (disableTracking)
            query = query.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(includeString))
            query = query.Include(includeString);

        if (predicate != null)
            query = query.Where(predicate);

        if (orderBy != null)
            query = orderBy(query);

        return await query.ToListAsync(cancellationToken);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await Set.AddAsync(entity, cancellationToken);
        await Db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Set.Update(entity);
        await Db.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Set.Remove(entity);
        await Db.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Set.AnyAsync(predicate, cancellationToken);
    }
}
