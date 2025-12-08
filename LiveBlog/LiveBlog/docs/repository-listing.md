### Уніфікований список: пагінація, сортування, фільтрація, include

Цей проєкт містить уніфіковані класи-запити для роботи зі списками сутностей та розширений репозиторій, що виконує фільтрацію/сортування/пагінацію на рівні БД (EF Core).

#### Класи запитів (`Models/Base/Requests.cs`)
- `SortDirection` — напрям сортування: `Asc`/`Desc`.
- `IPageableRequest` — інтерфейс пагінації: `Page`, `PageSize`.
- `ISortableRequest<TSort>` — інтерфейс типізованого сортування: `SortBy` (enum), `SortDirection`.
- `IFilterableRequest<TFilter>` — інтерфейс фільтрації: `Filter` (DTO).
- `PageableRequest`, `SortableRequest<TSort>`, `FilterableRequest<TFilter>`, `PagedSortedFilteredRequest<TSort,TFilter>` — готові обгортки, які можна використовувати напряму.

#### Результат пагінації (`Models/Base/PagedResult.cs`)
```
class PagedResult<T>
{
    IReadOnlyList<T> Items;
    int Total;   // загальна кількість записів без урахування skip/take
    int Page;    // номер сторінки (1..)
    int PageSize;// розмір сторінки
}
```

#### Розширений базовий репозиторій
- Інтерфейс: `Repositories/Base/IRepository.cs`
  - Існуючий метод:
    ```csharp
    Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity,bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string? includeString = null,
        bool disableTracking = true,
        CancellationToken ct = default)
    ```
  - Новий перевантажений метод з підтримкою множинних `include` і пагінації:
    ```csharp
    Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity,bool>>? predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
        IEnumerable<string>? includeStrings,
        int? skip,
        int? take,
        bool disableTracking,
        CancellationToken ct = default)
    ```
  - Підрахунок загальної кількості:
    ```csharp
    Task<int> CountAsync(Expression<Func<TEntity,bool>>? predicate = null, CancellationToken ct = default)
    ```

- Реалізація: `Repositories/Base/EfRepository.cs` — вся логіка виконується в БД (Where/OrderBy/Include/Skip/Take).

#### Домейн «Пости»
- Сортування: `Models/Posts/PostSort.cs` — `Id`, `Slug`, `CreatedAt`, `UpdatedAt`.
- Фільтр: `Models/Posts/PostFilter.cs`
  - `UserId` — фільтр за автором
  - `Query` — загальний пошук в `Slug` та `Content` (LIKE)
  - `SlugContains`, `ContentContains` — окремі пошуки
  - `DateFrom`, `DateTo` — фільтр за діапазоном створення

- Репозиторій постів: `Repositories/Posts`
  - Інтерфейс `IPostRepository` доповнено методом:
    ```csharp
    Task<PagedResult<PostEntity>> ListAsync(
        PagedSortedFilteredRequest<PostSort, PostFilter> request,
        IEnumerable<string>? includes = null,
        CancellationToken ct = default)
    ```
  - Реалізація `PostRepository.ListAsync(...)` будує `predicate` з `request.Filter`, застосовує `SortBy`/`SortDirection`, рахує `Total` через `CountAsync` і повертає `PagedResult<PostEntity>`; підтримуються множинні `Include`.

#### Сервіс постів (`Services/Posts`)
- `IPostService` має новий метод:
```csharp
Task<PagedResult<SmallPostResponse>> ListAsync(
    PagedSortedFilteredRequest<PostSort, PostFilter> request,
    CancellationToken ct = default)
```
- `PostService.ListAsync(request)` викликає репозиторій, за замовчуванням додає include `MediaFiles` і мапить у `SmallPostResponse`.
- Старий метод `ListAsync()` збережено для сумісності — тепер він делегує новому з дефолтними параметрами.

#### Контролер користувача (`Controllers/UserPostController.cs`)
- Екшен `Index` тепер формує `PagedSortedFilteredRequest<PostSort,PostFilter>` із query-параметрів і викликає новий сервісний метод:
```csharp
public async Task<IActionResult> Index(int page=1, int pageSize=10, string? sort="id_desc", string? q=null, CancellationToken ct = default)
{
    var sortLower = (sort ?? "id_desc").ToLowerInvariant();
    var req = new PagedSortedFilteredRequest<PostSort, PostFilter>
    {
        Page = page,
        PageSize = pageSize,
        SortBy = sortLower.Contains("slug") ? PostSort.Slug : PostSort.Id,
        SortDirection = sortLower.EndsWith("_asc") ? SortDirection.Asc : SortDirection.Desc,
        Filter = new PostFilter { Query = string.IsNullOrWhiteSpace(q) ? null : q.Trim() }
    };

    var result = await _postService.ListAsync(req, ct);
    ViewBag.Page = result.Page;
    ViewBag.PageSize = result.PageSize;
    ViewBag.Total = result.Total;
    ViewBag.Sort = sort;
    ViewBag.Query = q;
    return View(result.Items);
}
```

Поточне Razor-представлення `Views/UserPost/Index.cshtml` продовжує працювати (посилання сортування залишені у вигляді рядка `sort` для зворотної сумісності). За потреби можна оновити посилання на використання переліку `PostSort` та `SortDirection` явно.

#### Приклади використання

1) Отримати пости із пагінацією та сортуванням за датою створення (спадання):
```csharp
var req = new PagedSortedFilteredRequest<PostSort, PostFilter>
{
    Page = 2,
    PageSize = 20,
    SortBy = PostSort.CreatedAt,
    SortDirection = SortDirection.Desc,
    Filter = new PostFilter { UserId = currentUserId }
};
var page = await _postService.ListAsync(req, ct);
```

2) Пошук за фразою у `Slug` та `Content` з підвантаженням медіа (за замовчуванням):
```csharp
var req = new PagedSortedFilteredRequest<PostSort, PostFilter>
{
    Page = 1,
    PageSize = 10,
    SortBy = PostSort.Id,
    SortDirection = SortDirection.Desc,
    Filter = new PostFilter { Query = "asp.net" }
};
var page = await _postService.ListAsync(req, ct);
```

3) Додаткові include у репозиторії (якщо потрібно отримувати інші навігаційні сутності):
```csharp
var includes = new [] { nameof(PostEntity.MediaFiles), "Comments", "Likes" };
var pg = await _postRepository.ListAsync(req, includes, ct);
```

#### Нотатки щодо продуктивності
- Всі фільтри/сортування/пагінація застосовуються на рівні БД (EF Core генерує відповідний SQL).
- `disableTracking = true` використовується за замовчуванням у методах списків для швидшого читання.
- Для великих обсягів даних контролюйте глибину `Include` та розмір сторінки.
