namespace VkNet.Commands.Models;

/// <summary>
/// Аргументы для события исключения команды
/// </summary>
public class CommandExceptionArgs
{
    /// <summary>
    /// Полный текст команды, полученный от пользователя
    /// </summary>
    public string FullCommandText { get; init; } = "";
    
    /// <summary>
    /// Исключение, которое было получено от команды
    /// </summary>
    public System.Exception? CommandException { get; init; }
}