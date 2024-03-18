using VkNet.Abstractions;
using VkNet.Commands.Models;

namespace VkNet.Commands.Extensions;

/// <summary>
/// Класс с расширениями для работы команд
/// </summary>
public static class CommandExtensions
{
    /// <summary>
    /// Добавление процессора команд в VkApi
    /// </summary>
    /// <param name="vkApi">VkApi</param>
    /// <param name="configuration">Конфигурация процессора команд</param>
    /// <returns>Процессор команд</returns>
    public static CommandProcessor AddCommands(this IVkApi vkApi, CommandsConfiguration configuration)
    {
        configuration.VkApi = vkApi;
        var cmdProcessor = new CommandProcessor(configuration);
        
        return cmdProcessor;
    }
}