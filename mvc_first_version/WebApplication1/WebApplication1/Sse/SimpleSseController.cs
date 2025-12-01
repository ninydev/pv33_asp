using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers.Sse;


[ApiController]
[Route("sse/simple/notifications")]
public class SimpleSseController : ControllerBase
{
    private readonly ILogger _logger;
    public SimpleSseController(ILogger logger)
    {
        _logger = logger;
    }
    
    [HttpGet("subscribe")]
    public async Task Get(CancellationToken cancellationToken)
    {
        // 1. Устанавливаем правильный Content-Type
        Response.Headers.Append("Content-Type", "text/event-stream");
        
        // Эти заголовки иногда нужны, чтобы прокси не кешировали поток
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        // 2. Цикл жизни соединения
        // Мы держим соединение открытым, пока клиент не отключится (cancellationToken сработает)
        try 
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Эмуляция получения данных (например, из Redis, Channel или базы)
                var message = $"Время на сервере: {DateTime.Now.ToLongTimeString()}";

                // 3. Формирование сообщения по стандарту SSE
                // Формат: "data: <твое сообщение>\n\n"
                var sseMessage = $"data: {message}\n\n";
                
                // Если нужно отправить ID события или тип:
                // sseMessage = $"id: 123\nevent: update\ndata: {message}\n\n";

                // 4. Запись в поток ответа
                await Response.WriteAsync(sseMessage, cancellationToken);
                
                // ВАЖНО: FlushAsync заставляет отправить данные немедленно, 
                // не дожидаясь заполнения буфера
                await Response.Body.FlushAsync(cancellationToken);

                // Задержка (в реальности тут будет await какого-то события)
                await Task.Delay(2000, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Клиент закрыл вкладку или отвалился интернет.
            // Здесь можно логировать отключение пользователя.
            _logger.LogInformation("Клиент отключился");
        }
    }
}