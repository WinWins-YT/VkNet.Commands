using VkNet.Abstractions;

namespace VkNet.Commands.Models;

/// <summary>
/// Конфигурация процессора команд
/// </summary>
public class CommandsConfiguration
{
    /// <summary>
    /// Массив префиксов для команды
    /// </summary>
    public string[] Prefixes { get; set; } = Array.Empty<string>();
    
    /// <summary>
    /// Id группы в ВК
    /// </summary>
    public required ulong GroupId { get; set; }
    
    /// <summary>
    /// VkApi
    /// </summary>
    internal IVkApi? VkApi { get; set; }
}