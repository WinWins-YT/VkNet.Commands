namespace VkNet.Commands.Exceptions;

/// <summary>
/// Исключение команды
/// </summary>
public class CommandException : System.Exception
{
    /// <summary>
    /// Класс с командой, вызвавшей исключение
    /// </summary>
    public object? CommandClass { get; }

    /// <summary>
    /// Конструктор исключения команды
    /// </summary>
    /// <param name="message">Сообщения об исключении</param>
    /// <param name="inner">Внутреннее исключение</param>
    /// <param name="commandClass">Класс с командой, вызвавшей исключение</param>
    public CommandException(string message, System.Exception inner, object? commandClass) : base(message, inner)
    {
        CommandClass = commandClass;
    }
}