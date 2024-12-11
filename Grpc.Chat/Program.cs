using Grpc.ChatServer;
using Grpc.Core;
using Grpc.Net.Client;

using var channel = GrpcChannel.ForAddress("http://localhost:5085");

var client = new Chat.ChatClient(channel);

Console.Write("Name: ");
var name = Console.ReadLine();

var reply = client.EnterChat(new EnterRequest { Name = name });

var writeThread = new Thread(async () =>
{
    while (true)
    {
        await foreach (var message in reply.ResponseStream.ReadAllAsync())
        {
            Console.WriteLine(message.Message);
        }
    }
});

writeThread.Start();


while (true)
{
    var message = Console.ReadLine();

    Console.SetCursorPosition(0, Console.CursorTop - 1);
    ClearLine();

    client.SendMessage(new ChatMessage { Message = $"{DateTime.UtcNow}, {name}: {message}" });
}

void ClearLine()
{
    int currentLineCursor = Console.CursorTop;
    Console.SetCursorPosition(0, Console.CursorTop);
    Console.Write(new string(' ', Console.WindowWidth));
    Console.SetCursorPosition(0, currentLineCursor);
}