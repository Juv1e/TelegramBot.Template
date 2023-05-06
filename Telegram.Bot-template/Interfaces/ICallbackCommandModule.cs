using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Template.Interfaces;

public interface ICallbackCommandModule
{
    string Command { get; }
    Task<string> HandleCallbackCommandAsync(ITelegramBotClient botClient, Update e);
}