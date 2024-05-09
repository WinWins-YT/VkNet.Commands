using VkNet.Model;

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
    /// Объект сообщения
    /// </summary>
    public Message MessageObject { get; set; }

    /// <summary>
    /// Конструктор исключения команды
    /// </summary>
    /// <param name="message">Сообщения об исключении</param>
    /// <param name="inner">Внутреннее исключение</param>
    /// <param name="commandClass">Класс с командой, вызвавшей исключение</param>
    /// <param name="messageObject">Объект сообщения</param>
    public CommandException(string message, System.Exception inner, object? commandClass, Message messageObject) : base(message, inner)
    {
        CommandClass = commandClass;
        MessageObject = messageObject;
    }
}