namespace WebApplication1.Areas.MyTask.DTO;

/// <summary>
/// Ответ при успешном создании задачи.
/// </summary>
public class TaskCreatedResponseDto
{
    public int Id { get; set; }

    /// <summary>
    /// Абсолютный или относительный URL на созданный ресурс (опционально).
    /// </summary>
    public string? Location { get; set; }
}
