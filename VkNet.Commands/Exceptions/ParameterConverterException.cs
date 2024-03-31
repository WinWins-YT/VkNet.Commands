namespace VkNet.Commands.Exceptions;

/// <summary>
/// Исключение конвертера параметров
/// </summary>
public class ParameterConverterException : System.Exception
{
    /// <summary>
    /// Конструктор исключения конструктора параметров
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <param name="innerException">Внутреннее исключение</param>
    public ParameterConverterException(string? message, System.Exception? innerException) : base(message, innerException)
    {
    }
}