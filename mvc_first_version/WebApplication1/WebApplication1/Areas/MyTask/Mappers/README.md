Папка: Mappers

Назначение
- Маппинг между `Entities` и `DTO`: ручные мапперы или профили AutoMapper.

Рекомендации
- Для AutoMapper создайте профиль `TaskMappingProfile` и регистрируйте его в `Program.cs` через `AddAutoMapper` (при использовании).
- Маппинг не должен содержать побочных эффектов и бизнес‑логики.

Пример профиля (эскиз)
```csharp
using AutoMapper;
using WebApplication1.Areas.Task.Entities;
using WebApplication1.Areas.Task.DTO;

namespace WebApplication1.Areas.Task.Mappers;

public class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        CreateMap<TaskEntity, TaskItemDto>();
        CreateMap<TaskEntity, TaskDetailsDto>();
        CreateMap<TaskCreateDto, TaskEntity>();
        CreateMap<TaskUpdateDto, TaskEntity>();
    }
}
```
