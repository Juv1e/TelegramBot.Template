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
    private static readonly Dictionary<string, ICallbackCommandModule> CallbackCommandModules = new();
    
    static async Task Main()
    {
        RegisterCommandModules();
        RegisterCallbackCommandModules();
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
                await Task.Run(async () =>
                {
                    string? message = new[] {e.Message.Text, e.Message.Caption}
                        .FirstOrDefault(s => !string.IsNullOrWhiteSpace(s)) ?? "";
                    Console.WriteLine($"[{e.Message.Chat.Id} - @{e.Message.From.Username}] {message}");
                    if (e.Message.Type == MessageType.Text || e.Message.Type == MessageType.Photo ||
                        e.Message.Type == MessageType.Video && message.Length > 0)
                    {
                        var command = message.Split(' ')[0];

                        if (CommandModules.TryGetValue(command, out var module) &&
                            !AntiFlood.IsFlood(e.Message.Chat.Id))
                        {
                            var response = await module.HandleCommandAsync(botClient, e);
                            if (!string.IsNullOrWhiteSpace(response))
                            {
                                await botClient.SendTextMessageAsync(e.Message.Chat.Id, response);
                            }
                        }
                    }
                });
                break;
            }
            case UpdateType.CallbackQuery:
            {
                await Task.Run(async () =>
                {
                    var command = e.CallbackQuery.Data;
                    if (CallbackCommandModules.TryGetValue(command, out var module))
                    {
                        if (AntiFlood.IsFlood(e.CallbackQuery.Message.Chat.Id))
                        {
                            await botClient.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id,
                                "Too many requests in a short period of time. Please wait a little longer.");
                            return;
                        }

                        var response = await module.HandleCallbackCommandAsync(botClient, e);
                        if (!string.IsNullOrWhiteSpace(response))
                        {
                            await botClient.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, response);
                        }
                    }
                });
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
        var commandModuleTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(ICommandModule).IsAssignableFrom(t));
        
        foreach (var type in commandModuleTypes)
        {
            var commandModule = (ICommandModule)Activator.CreateInstance(type);
            CommandModules[commandModule.Command] = commandModule;
        }
    }
    private static void RegisterCallbackCommandModules()
    {
        var commandModuleTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(ICallbackCommandModule).IsAssignableFrom(t));
        
        foreach (var type in commandModuleTypes)
        {
            var commandModule = (ICallbackCommandModule)Activator.CreateInstance(type);
            CallbackCommandModules[commandModule.Command] = commandModule;
        }
    }
}