namespace VkNet.Commands.Exceptions;

/// <summary>
/// Исключение, когда метод команды неверный (отсутствуют необходимые параметры)
/// </summary>
/// <param name="message">Сообщение исключения</param>
public class InvalidMethodException(string? message) : System.Exception(message);