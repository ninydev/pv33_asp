Папка: Exceptions (область MyTask)

Назначение
- Прикладные (доменные) исключения для области задач. Используются в `Services` и при необходимости обрабатываются в middleware/фильтрах.

Состав
- `MyTaskErrorCode` — перечисление кодов ошибок: `Unknown`, `NotFound`, `ValidationFailed`, `Conflict`, `Forbidden`, `Concurrency`.
- `MyTaskException` — базовый тип для всех исключений области. Имеет свойства:
  - `ErrorCode` — код ошибки домена;
  - `Errors` — опциональные детали валидации (`Dictionary<string, string[]>`).
- Конкретные исключения:
  - `TaskNotFoundException` — ресурс не найден (обычно 404).
  - `TaskValidationException` — бизнес-валидация/правила (400). Поддерживает передачу ошибок по полям.
  - `TaskConflictException` — конфликт бизнес-инвариантов/дубликаты (409).
  - `TaskForbiddenException` — запрет по правам/ролям для действия (403).
  - `TaskConcurrencyException` — конкурентное обновление (409).
- `MyTaskExceptionMapping` — helper для маппинга исключений в:
  - рекомендуемый HTTP-статус (`GetHttpStatusCode`),
  - компактную структуру ошибки `ErrorInfo`, совместимую с `ProblemDetails`.

Рекомендации по использованию
- Сервисный слой (например, `TaskService`) может выбрасывать конкретные исключения при нарушении правил.
- В контроллерах/мидлваре перехватывайте `MyTaskException` и используйте `MyTaskExceptionMapping`
  для построения ответа.

Примеры
```csharp
// В сервисе
var entity = await _repo.GetByIdAsync(id, includeAssignee: true, ct);
if (entity == null)
    throw new TaskNotFoundException(id);

if (dto.DueDate.HasValue && dto.DueDate < DateTimeOffset.UtcNow)
    throw new TaskValidationException("DueDate", "Срок не может быть в прошлом.");
```

```csharp
// В middleware/фильтре (эскиз)
catch (Exception ex) {
    var status = MyTaskExceptionMapping.GetHttpStatusCode(ex);
    var payload = MyTaskExceptionMapping.GetErrorInfo(ex);
    context.Response.StatusCode = status;
    await context.Response.WriteAsJsonAsync(payload, cancellationToken: ct);
}
```

Заметки
- Исключения не зависят от ASP.NET и могут использоваться в любом хосте.
- Для MVC ModelState-ошибок используйте стандартные механизмы; `TaskValidationException` — для бизнес-правил.
