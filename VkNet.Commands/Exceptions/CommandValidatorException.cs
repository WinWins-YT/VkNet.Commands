namespace VkNet.Commands.Exceptions;

/// <summary>
/// Возникает, когда произошла ошибка при валидации команды
/// </summary>
public class CommandValidatorException : System.Exception
{
    /// <summary>
    /// Возникает, когда произошла ошибка при валидации команды
    /// </summary>
    /// <param name="message">Сообщение</param>
    public CommandValidatorException(string? message) : base(message)
    {
    }

    /// <summary>
    /// Возникает, когда произошла ошибка при валидации команды
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="innerException">Дополнительное исключение</param>
    public CommandValidatorException(string? message, System.Exception? innerException) : base(message, innerException)
    {
    }
}