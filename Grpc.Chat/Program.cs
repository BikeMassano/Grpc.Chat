using Grpc.ChatServer;
using Grpc.Core;
using Grpc.Net.Client;

// Канал подключения к серверу
using var channel = GrpcChannel.ForAddress("http://localhost:5085");

var client = new Chat.ChatClient(channel);

Console.Write("Имя пользователя: ");
var name = Console.ReadLine();

var reply = client.EnterChat(new EnterRequest { Name = name });

// Создаём поток, в котором принимаются сообщения с сервера
var messageReceivingTask = Task.Run(async () =>
{
    try
    {
        await foreach (var message in reply.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine(message.Message);
        }
    }
    catch (RpcException e)
    {
        Console.WriteLine($"Ошибка при получении сообщения: {e.Message}");
    }
});


while (true)
{
    // Считываем значение из консоли
    var message = Console.ReadLine();

    // Устанавливаем курсор
    Console.SetCursorPosition(0, Console.CursorTop - 1);
    ClearLine();
    // Отправляем сообщение на сервер
    client.SendMessage(new ChatMessage { Message = $"{DateTime.UtcNow}, {name}: {message}" });
}

void ClearLine()
{
    int currentLineCursor = Console.CursorTop;
    Console.SetCursorPosition(0, Console.CursorTop);
    Console.Write(new string(' ', Console.WindowWidth));
    Console.SetCursorPosition(0, currentLineCursor);
}
