using System.Collections.Generic;
using WebApplication1.Entities;
using WebApplication1.ViewModel;

namespace WebApplication1.Mappers;

/// <summary>
/// Мапер для користувача: конвертує MyIdentityUserEntity у полегшену модель ShowUserViewModel.
/// Дозволяє уникати циклічних залежностей (User -> Posts -> User ...).
/// </summary>
public static class MyIndentityuserMapper
{
    /// <summary>
    /// Перетворює сутність користувача у спрощений вигляд.
    /// </summary>
    public static ShowUserViewModel ToShowViewModel(this MyIdentityUserEntity user)
    {
        if (user == null) return null;
        return new ShowUserViewModel
        {
            Id = user.Id,
            UserName = user.UserName
        };
    }

    /// <summary>
    /// Перетворює колекцію сутностей користувачів у колекцію ShowUserViewModel.
    /// </summary>
    public static IEnumerable<ShowUserViewModel> ToShowViewModels(this IEnumerable<MyIdentityUserEntity> users)
    {
        if (users == null) yield break;
        foreach (var u in users)
        {
            yield return ToShowViewModel(u);
        }
    }
}