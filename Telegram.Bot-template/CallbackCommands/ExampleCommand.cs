using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Template.Interfaces;

namespace TelegramBot.Template.CallbackCommands;

public class ExampleCommand : ICallbackCommandModule
{
    public string Command => "example_callback";

    public async Task<string> HandleCallbackCommandAsync(ITelegramBotClient botClient, Update e)
    {
        var chatId = e.CallbackQuery.Message.Chat.Id;
        return $"Button from {chatId}";
    }
}