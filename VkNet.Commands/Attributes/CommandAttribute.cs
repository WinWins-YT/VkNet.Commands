namespace VkNet.Commands.Attributes;

/// <summary>
/// Аттрибут команды
/// </summary>
/// <param name="name">Имя команды</param>
[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute(string name) : Attribute
{
    /// <summary>
    /// Имя команды
    /// </summary>
    public string Name { get; init; } = name;
}