namespace LiveBlog.Areas.Sse;

using System;
using System.Threading.Channels;

public class SseClient
{
    public string ConnectionId { get; } = Guid.NewGuid().ToString(); // Уникальный ID вкладки/соединения
    public string? UserId { get; init; } // ID пользователя (null если аноним)
    
    // Канал для отправки сообщений этому конкретному клиенту
    // Это как "почтовый ящик", куда сервис кладет сообщения
    public System.Threading.Channels.Channel<string> Channel { get; } = System.Threading.Channels.Channel.CreateUnbounded<string>();
}