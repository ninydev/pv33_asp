using LiveBlog.Data;
using LiveBlog.Models.Base;
using LiveBlog.Models.Posts;
using LiveBlog.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LiveBlog.Repositories.Posts;

/// <summary>
/// Реалізація репозиторію для роботи з дописами.
/// Інкапсулює специфічні запити до EF Core для сутності <see cref="PostEntity"/>.
/// </summary>
public class PostRepository : EfRepository<PostEntity>, IPostRepository
{
    public PostRepository(ApplicationDbContext db) : base(db)
    {
    }

    /// <inheritdoc />
    public Task<PostEntity?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return Set.AsNoTracking().FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
    }

    /// <inheritdoc />
    public Task<PostEntity?> GetWithMediaByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return Set.Include(p => p.MediaFiles).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PagedResult<PostEntity>> ListAsync(
        PagedSortedFilteredRequest<PostSort, PostFilter> request,
        IEnumerable<string>? includes = null,
        CancellationToken cancellationToken = default)
    {
        // Підготовка предиката фільтрації
        Expression<Func<PostEntity, bool>>? predicate = null;

        if (request.Filter is { } f)
        {
            predicate = p => true; // стартова завжди істина, щоб зручно AND'ити

            if (!string.IsNullOrWhiteSpace(f.UserId))
                predicate = And(predicate, p => p.UserId == f.UserId);

            if (!string.IsNullOrWhiteSpace(f.SlugContains))
            {
                var term = f.SlugContains!.Trim();
                predicate = And(predicate, p => EF.Functions.Like(p.Slug, $"%{term}%"));
            }

            if (!string.IsNullOrWhiteSpace(f.ContentContains))
            {
                var term = f.ContentContains!.Trim();
                predicate = And(predicate, p => EF.Functions.Like(p.Content, $"%{term}%"));
            }

            if (!string.IsNullOrWhiteSpace(f.Query))
            {
                var q = f.Query!.Trim();
                predicate = And(predicate, p => EF.Functions.Like(p.Slug, $"%{q}%") || EF.Functions.Like(p.Content, $"%{q}%"));
            }

            if (f.DateFrom.HasValue)
            {
                var from = f.DateFrom.Value;
                predicate = And(predicate, p => p.CreatedAt >= from);
            }
            if (f.DateTo.HasValue)
            {
                var to = f.DateTo.Value;
                predicate = And(predicate, p => p.CreatedAt <= to);
            }
        }

        // Побудова сортування
        Func<IQueryable<PostEntity>, IOrderedQueryable<PostEntity>>? orderBy = null;
        if (request.SortBy.HasValue)
        {
            var desc = request.SortDirection == SortDirection.Desc;
            orderBy = q => request.SortBy switch
            {
                PostSort.Slug => desc ? q.OrderByDescending(p => p.Slug).ThenByDescending(p => p.Id) : q.OrderBy(p => p.Slug).ThenBy(p => p.Id),
                PostSort.CreatedAt => desc ? q.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id) : q.OrderBy(p => p.CreatedAt).ThenBy(p => p.Id),
                PostSort.UpdatedAt => desc ? q.OrderByDescending(p => p.UpdatedAt).ThenByDescending(p => p.Id) : q.OrderBy(p => p.UpdatedAt).ThenBy(p => p.Id),
                _ => desc ? q.OrderByDescending(p => p.Id) : q.OrderBy(p => p.Id)
            };
        }
        else
        {
            // Сортування за умовчанням — за Id спадаючим (новіші зверху)
            orderBy = q => q.OrderByDescending(p => p.Id);
        }

        var page = Math.Max(1, request.Page);
        var size = Math.Max(1, request.PageSize);
        var skip = (page - 1) * size;

        // Загальна кількість без урахування Skip/Take
        var total = await CountAsync(predicate, cancellationToken);

        var items = await ListAsync(
            predicate,
            orderBy,
            includes,
            skip,
            size,
            disableTracking: true,
            cancellationToken);

        return new PagedResult<PostEntity>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = size
        };
    }

    private static Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
    {
        var param = Expression.Parameter(typeof(T));
        var body = Expression.AndAlso(
            Expression.Invoke(left, param),
            Expression.Invoke(right, param));
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}
