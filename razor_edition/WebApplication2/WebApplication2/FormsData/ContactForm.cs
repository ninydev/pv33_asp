using System.ComponentModel.DataAnnotations;

public class ContactForm
{
    [Required(ErrorMessage = "Имя обязательное поле")]
    [Display(Name = "Имя")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Имя должно быть от 2 до 50 символов")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Обязательно Введите email")]
    [Display(Name = "Email")]   
    [EmailAddress(ErrorMessage = "Некорректный адрес электронной почты")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Без сообения я не оптавлю сообщение")]
    [Display(Name = "Введите сообщение")]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Сообщение должно быть от 10 до 500 символов")]
    public string Message { get; set; }

    [Range(18, 99, ErrorMessage = "Возраст должен быть от 18 до 99 лет")]
    public int Age { get; set; }
}