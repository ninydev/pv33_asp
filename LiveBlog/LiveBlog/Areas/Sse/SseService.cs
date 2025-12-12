using System.Text.Json.Serialization;

namespace LiveBlog.Areas.Sse;


using System.Collections.Concurrent;
using System.Text.Json;

public class SseService
{
    
    private readonly ILogger<SseService> _logger;
    public SseService(ILogger<SseService> logger)
    {
        _logger = logger;
    }
    
    // Потокобезопасный словарь для хранения всех активных соединений
    // Ключ: ConnectionId, Значение: SseClient
    private readonly ConcurrentDictionary<string, SseClient> _clients = new();

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
    };

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
        _logger.LogInformation($"Send to all: {message}");
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
        // Правило:
        // - event: имя типа payload (Type.Name)
        // - id: текущий Unix TimeStamp в миллисекундах
        // - data: строка как есть, объект — JSON; при неудаче сериализации — ToString()

        string eventName;
        string dataLine;

        if (payload is null)
        {
            eventName = "null";
            dataLine = "null";
        }
        else if (payload is string s)
        {
            eventName = payload.GetType().Name; // "String"
            dataLine = s;
        }
        else
        {
            eventName = payload.GetType().Name;
            try
            {
                dataLine = JsonSerializer.Serialize(payload, _jsonOptions);
            }
            catch
            {
                dataLine = payload?.ToString() ?? string.Empty;
            }
        }

        // id — Unix time в мс
        var id = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

        // Если в данных есть переносы строк — по спецификации SSE каждый рядок должен иметь префикс "data: "
        // Разобьём на строки и соберём корректно.
        var lines = dataLine.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        var sb = new System.Text.StringBuilder();
        sb.Append("event: ").AppendLine(eventName);
        sb.Append("id: ").AppendLine(id);
        foreach (var line in lines)
        {
            sb.Append("data: ").AppendLine(line);
        }
        sb.AppendLine(); // пустая строка — разделитель событий
        return sb.ToString();
    }
    
    
    private string FormatMessageIncludeType<T>(T payload)
    {
        // Обработка null
        if (payload is null) return "null";

        // Если это уже строка — отправляем как есть
        if (payload is string s) return s;

        // Для любых других объектов добавляем тип в сообщение и сериализуем как JSON
        try
        {
            var type = payload.GetType();
            var envelope = new
            {
                type = type.Name,          // Имя класса, например LikePostNotification
                date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                data = (object)payload     // Сам объект
            };
            return JsonSerializer.Serialize(envelope, _jsonOptions);
        }
        catch
        {
            // Фолбэк: отправляем ToString(), но сохраняем тип в JSON-обёртке
            var type = payload.GetType();
            var envelope = new
            {
                type = type.Name,
                date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                data = payload?.ToString() ?? string.Empty
            };
            return JsonSerializer.Serialize(envelope, _jsonOptions);
        }
    }
}