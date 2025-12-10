namespace LiveBlog.Areas.Sse;


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("sse/")]
public class SseController : ControllerBase
{
    private readonly SseService _sseService;
    private readonly ILogger<SseController> _logger;

    public SseController(SseService sseService, ILogger<SseController> logger)
    {
        _sseService = sseService;
        _logger = logger;
    }

    [HttpGet("connect")]
    // [Authorize] // Раскомментируй, если пускаем только авторизованных
    public async Task Connect(CancellationToken cancellationToken)
    {
        Response.Headers.Append("Content-Type", "text/event-stream");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        // 1. Определяем юзера (если авторизован)
        // Ищем ClaimTypes.NameIdentifier (обычно это ID) или ClaimTypes.Name
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.Identity?.Name;

        if (userId != null)
        {
            await _sseService.SendToAllAsync("User connected: " + userId + "");
        }
        else
        {
            await _sseService.SendToAllAsync("Anonymous user connected");
        }

        // 2. Регистрируем клиента в сервисе
        var client = _sseService.AddClient(userId);

        // Отправим приветственное сообщение только ему
        await client.Channel.Writer.WriteAsync($"data: Connected! Your ConnId: {client.ConnectionId}\n\n");

        try
        {
            // 3. Читаем из канала, пока соединение живое
            // Этот цикл будет висеть, пока клиент подключен
            await foreach (var message in client.Channel.Reader.ReadAllAsync(cancellationToken))
            {
                // Если сообщение уже содержит корректный SSE‑фрейм (начинается с "event:" или "id:" или содержит строки "data:"),
                // передаём его как есть. Иначе — оборачиваем в безымянное событие с префиксом data: ... \n\n
                var trimmed = message?.TrimStart();
                var looksLikeSseFrame = !string.IsNullOrEmpty(trimmed) &&
                                        (trimmed.StartsWith("event:") || trimmed.StartsWith("id:") || trimmed.StartsWith("data:"));

                if (looksLikeSseFrame)
                {
                    await Response.WriteAsync(message, cancellationToken);
                }
                else
                {
                    await Response.WriteAsync($"data: {message}\n\n", cancellationToken);
                }

                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Нормальное отключение клиента
            _logger.LogInformation($"Client {client.ConnectionId} {client.UserId} disconnected");
        }
        finally
        {
            // 4. Обязательно удаляем клиента из списка при разрыве
            _sseService.RemoveClient(client.ConnectionId);
        }
    }
}