using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Template.Interfaces;

namespace TelegramBot.Template.Commands;

public class HelpCommand : ICommandModule
{
    public string Command => "/help";
    public string Description => "Returns all available commands";
    public async Task<string> HandleCommandAsync(ITelegramBotClient botClient, Update e)
    {
        var commands = GetAvailableCommands(GetCommands());
        return $"Available commands:\n\n{commands}";
    }
    private static List<ICommandModule> GetCommands()
    {
        var commandModules = new List<ICommandModule>();
        
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
        
        foreach (var type in types.Where(t => t.IsClass && !t.IsAbstract && typeof(ICommandModule).IsAssignableFrom(t)))
        {
            var commandModule = (ICommandModule)Activator.CreateInstance(type);
            
            commandModules.Add(commandModule);
        }
    
        return commandModules;
    }
    private static string GetAvailableCommands(List<ICommandModule> commands)
    {
        var messageBuilder = new StringBuilder();
    
        foreach (var command in commands)
        {
            messageBuilder.AppendLine($"• {command.Command} - {command.Description}");
        }
    
        return messageBuilder.ToString();
    }
}