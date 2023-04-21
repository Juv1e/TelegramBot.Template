using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Template;
using TelegramBot.Template.Interfaces;

class Program
{
    public static ITelegramBotClient botClient = new TelegramBotClient("TELEGRAM_TOKEN");
    private static readonly Dictionary<string, ICommandModule> CommandModules = new();

    static async Task Main()
    {
        RegisterCommandModules();
        CancellationTokenSource cts = new CancellationTokenSource();
        ReceiverOptions receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { }
        };
        botClient.StartReceiving(
            HandleUpdatesAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken: cts.Token);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
    }
     private static async Task HandleUpdatesAsync(ITelegramBotClient botClient, Update e, CancellationToken cancellationToken)
    {
        switch (e.Type)
        {
            case UpdateType.Message:
            {
                string? message = new[] { e.Message.Text, e.Message.Caption }
                    .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s)) ?? "";
                Console.WriteLine($"[{e.Message.Chat.Id} - @{e.Message.From.Username}] {message}");
                if (e.Message.Type == MessageType.Text || e.Message.Type == MessageType.Photo ||
                    e.Message.Type == MessageType.Video && message.Length > 0)
                {
                    var command = message.Split(' ')[0];
                    
                    if (CommandModules.TryGetValue(command, out var module) && !AntiFlood.IsFlood(e.Message.Chat.Id))
                    {
                        var response = await module.HandleCommandAsync(botClient, e);
                        if (!string.IsNullOrWhiteSpace(response))
                        {
                            await botClient.SendTextMessageAsync(e.Message.Chat.Id, response);
                        }
                    }
                }

                break;
            }
            case UpdateType.CallbackQuery:
            {
                var data = e.CallbackQuery.Data;
                switch (data)
                {
                    case "data1":
                        //обработка кнопки с data1
                        break;
                    case "data2":
                        //обработка кнопки с data2
                        break;
                }
                break;
            }
        }
    }
    static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
    private static void RegisterCommandModules()
    {
        // Получаем все классы, реализующие интерфейс ICommandModule
        var commandModuleTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(ICommandModule).IsAssignableFrom(t));

        // Создаем экземпляры классов и регистрируем их в словаре CommandModules
        foreach (var type in commandModuleTypes)
        {
            var commandModule = (ICommandModule)Activator.CreateInstance(type);
            CommandModules[commandModule.Command] = commandModule;
        }
    }
}