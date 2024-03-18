using VkNet.Abstractions;

namespace VkNet.Commands.Models;

public class CommandsConfiguration
{
    public string[] Prefixes { get; set; } = Array.Empty<string>();
    public required ulong GroupId { get; set; }
    internal IVkApi? VkApi { get; set; }
}