// Профиль AutoMapper обёрнут в условную компиляцию, чтобы не ломать сборку,
// если пакет AutoMapper ещё не добавлен. Для включения удалите директивы или
// определите символ компиляции AUTO_MAPPER и добавьте пакет AutoMapper.
#if AUTO_MAPPER
using AutoMapper;
using WebApplication1.Areas.MyTask.DTO;
using WebApplication1.Areas.MyTask.Entities;
using TaskStatus = WebApplication1.Areas.MyTask.Entities.TaskStatus;

namespace WebApplication1.Areas.MyTask.Mappers;

/// <summary>
/// Профиль AutoMapper для маппинга между TaskEntity и DTO.
/// </summary>
public class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        // Entity -> DTO (read)
        CreateMap<TaskEntity, TaskListItemDto>()
            .ForMember(d => d.AssigneeUserName, o => o.MapFrom(s => s.Assignee != null ? s.Assignee.UserName : null));

        CreateMap<TaskEntity, TaskDetailsDto>()
            .ForMember(d => d.AssigneeUserName, o => o.MapFrom(s => s.Assignee != null ? s.Assignee.UserName : null))
            .ForMember(d => d.AssigneeEmail, o => o.MapFrom(s => s.Assignee != null ? s.Assignee.Email : null));

        // Create/Patch/Update -> Entity (write)
        CreateMap<TaskCreateRequestDto, TaskEntity>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Status, o => o.MapFrom(_ => TaskStatus.New))
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore());

        CreateMap<TaskUpdateRequestDto, TaskEntity>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore());

        // Для PATCH: маппим только непустые поля
        CreateMap<TaskPatchRequestDto, TaskEntity>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
#endif
