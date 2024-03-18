namespace VkNet.Commands.Exceptions;

/// <summary>
/// Исключение, когда нет конвертора для типа в методе команды
/// </summary>
/// <param name="message">Сообщение исключения</param>
public class NoParameterConverterException(string? message) : System.Exception(message);