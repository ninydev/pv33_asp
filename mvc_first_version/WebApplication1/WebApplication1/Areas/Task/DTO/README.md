Папка: DTO (Data Transfer Objects)

Назначение
- Классы-модели для обмена данными между слоями и внешними клиентами (контроллеры, API, UI). Не содержат бизнес‑логики.

Типовые DTO
- Входные (запросы): `TaskCreateDto`, `TaskUpdateDto`, `TaskFilterDto`.
- Выходные (ответы): `TaskItemDto`, `TaskDetailsDto`, `TaskPagedListDto`.

Правила
- Не размещайте в DTO доменную логику или зависимости от EF Core.
- Избегайте утечек сущностей — не возвращайте `TaskEntity` напрямую из контроллеров.
- Валидируйте входные DTO атрибутами data annotations при необходимости.
