using VkNet.Commands.Abstractions;
using VkNet.Commands.Attributes;
using VkNet.Model;

namespace VkNet.Commands.Exceptions;

/// <summary>
/// Возникает при неудачной валидации
/// </summary>
public class FailedValidationException : System.Exception
{
    /// <summary>
    /// Валидатор, который не прошел проверку
    /// </summary>
    public CommandValidatorAttribute Validator { get; set; }

    /// <summary>
    /// Объект сообщения
    /// </summary>
    public Message MessageObject { get; set; }

    /// <summary>
    /// Возникает при неудачной валидации
    /// </summary>
    public FailedValidationException(string? message, CommandValidatorAttribute validator, Message messageObject) : base(message)
    {
        Validator = validator;
        MessageObject = messageObject;
    }
}