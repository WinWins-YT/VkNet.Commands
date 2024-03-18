using VkNet.Abstractions;
using VkNet.Commands.Models;

namespace VkNet.Commands.Extensions;

public static class CommandExtensions
{
    public static CommandProcessor AddCommands(this IVkApi vkApi, CommandsConfiguration configuration)
    {
        configuration.VkApi = vkApi;
        var cmdProcessor = new CommandProcessor(configuration);
        
        return cmdProcessor;
    }
}