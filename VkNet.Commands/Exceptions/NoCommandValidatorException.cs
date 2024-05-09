namespace VkNet.Commands.Exceptions;

/// <summary>
/// Возникает, когда нет валидатора команд для атрибута
/// </summary>
public class NoCommandValidatorException : System.Exception
{
    /// <summary>
    /// Возникает, когда нет валидатора команд для атрибута
    /// </summary>
    /// <param name="message">Сообщение</param>
    public NoCommandValidatorException(string? message) : base(message)
    {
    }
}