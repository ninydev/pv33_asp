namespace LiveBlog.Areas.Sse;


using System.Collections.Concurrent;
using System.Text.Json;

public class SseService
{
    // Потокобезопасный словарь для хранения всех активных соединений
    // Ключ: ConnectionId, Значение: SseClient
    private readonly ConcurrentDictionary<string, SseClient> _clients = new();
    private readonly JsonSerializerOptions _jsonOptions = new();

    // 1. Метод регистрации нового соединения
    public SseClient AddClient(string? userId)
    {
        var client = new SseClient { UserId = userId };
        _clients.TryAdd(client.ConnectionId, client);
        return client;
    }

    // 2. Метод удаления соединения (когда клиент отвалился)
    public void RemoveClient(string connectionId)
    {
        _clients.TryRemove(connectionId, out _);
    }

    // 3. Отправка ВСЕМ (Broadcast)
    public async Task SendToAllAsync(string message)
    {
        foreach (var client in _clients.Values)
        {
            await client.Channel.Writer.WriteAsync(message);
        }
    }

    // Перегрузка: принимает любой объект и решает, как его отправлять
    public Task SendToAllAsync<T>(T payload)
    {
        var message = FormatMessage(payload);
        return SendToAllAsync(message);
    }

    // 4. Отправка КОНКРЕТНОМУ пользователю (по ID или Имени)
    // У пользователя может быть открыто 3 вкладки, отправим во все
    public async Task SendToUserAsync(string userId, string message)
    {
        var userConnections = _clients.Values.Where(c => c.UserId == userId);
        
        foreach (var client in userConnections)
        {
            await client.Channel.Writer.WriteAsync(message);
        }
    }

    // Перегрузка: принимает любой объект
    public Task SendToUserAsync<T>(string userId, T payload)
    {
        var message = FormatMessage(payload);
        return SendToUserAsync(userId, message);
    }

    private string FormatMessage<T>(T payload)
    {
        // Обработка null
        if (payload is null) return "null";

        // Если это уже строка — отправляем как есть
        if (payload is string s) return s;

        // Пытаемся сериализовать в JSON
        try
        {
            return JsonSerializer.Serialize(payload, _jsonOptions);
        }
        catch
        {
            // Фолбэк на ToString()
            return payload?.ToString() ?? string.Empty;
        }
    }
}