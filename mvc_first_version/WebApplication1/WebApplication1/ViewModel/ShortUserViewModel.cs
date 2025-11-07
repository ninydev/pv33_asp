namespace WebApplication1.ViewModel;

/// <summary>
/// Спрощена модель користувача для відображення (уникаємо циклічних залежностей).
/// Містить лише значущі поля для UI.
/// </summary>
public class ShortUserViewModel
{
    // Ідентифікатор користувача (IdentityUser.Id — рядок)
    public string Id { get; set; }
    
    // Ім'я користувача (логін)
    public string UserName { get; set; }
}