using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Template.Interfaces;

namespace TelegramBot.Template.Commands;

public class PingCommand : ICommandModule
{
    public string Command => "/ping";
    public string Description => "Example command";
    public async Task<string> HandleCommandAsync(ITelegramBotClient botClient, Update e)
    {
        return "pong";
    }
}