using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Template.Interfaces;

public interface ICommandModule
{
    string Command { get; }
    string Description { get; }
    Task<string> HandleCommandAsync(ITelegramBotClient botClient, Update e);
}