using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LiveBlog.Models.IdentityUser;

namespace LiveBlog.Models.Base;

/// <summary>
/// Базова сутність для всіх моделей БД.
/// Містить уніфіковані поля ідентифікатора, автора створення та часові мітки.
/// Зауваження щодо SQLite: зберігаємо час у форматі UTC у полях <see cref="CreatedAt"/> та <see cref="UpdatedAt"/>.
/// Актуальне значення задається на рівні застосунку (конструктор), оскільки SQLite має обмеження щодо генерації значень та обчислюваних колонок.
/// </summary>
public class BaseEntity
{
    /// <summary>
    /// Первинний ключ сутності.
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Ідентифікатор користувача (автора створення запису) з таблиці користувачів Identity.
    /// </summary>
    [ForeignKey("CreatedBy")] 
    public string UserId { get; set; }
    
    /// <summary>
    /// Навігаційна властивість до сутності користувача, який створив запис.
    /// </summary>
    public MyIdentityUserEntity CreatedBy { get; set; }
    
    /// <summary>
    /// Конструктор встановлює часові мітки у UTC на момент створення сутності в пам'яті.
    /// Зверніть увагу: для SQLite час встановлюється застосунком, а не БД.
    /// </summary>
    public BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Дата та час створення (UTC). Для SQLite заповнюється застосунком.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата та час останнього оновлення (UTC). Для SQLite оновлюється застосунком.
    /// Null означає, що запис ще не оновлювався після створення.
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime? UpdatedAt { get; set; }
}