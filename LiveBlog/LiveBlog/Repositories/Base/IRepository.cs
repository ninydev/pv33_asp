using System.Linq.Expressions;
using LiveBlog.Models.Base;

namespace LiveBlog.Repositories.Base;

/// <summary>
/// Базовий узагальнений репозиторій для CRUD-операцій над сутностями.
/// Призначений для інкапсуляції доступу до БД та повторного використання.
/// </summary>
/// <typeparam name="TEntity">Тип сутності, що успадковує <see cref="BaseEntity"/>.</typeparam>
public interface IRepository<TEntity> where TEntity : BaseEntity
{
    /// <summary>
    /// Повертає сутність за ідентифікатором або <c>null</c>, якщо не знайдено.
    /// </summary>
    Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Повертає колекцію сутностей з опціональним фільтром, сортуванням та включеннями.
    /// </summary>
    Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Додає нову сутність до БД та зберігає зміни.
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Оновлює наявну сутність та зберігає зміни.
    /// </summary>
    Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Видаляє сутність та зберігає зміни.
    /// </summary>
    Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Перевіряє наявність сутності, що задовольняє умову.
    /// </summary>
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}
