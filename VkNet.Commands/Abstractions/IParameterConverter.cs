namespace VkNet.Commands.Abstractions;

/// <summary>
/// Интерфейс для конвертора параметров
/// </summary>
public interface IParameterConverter {}

/// <summary>
/// Интерфейс для конвертора параметров
/// </summary>
/// <typeparam name="T">Тип в который конвертор преобразует</typeparam>
public interface IParameterConverter<out T> : IParameterConverter
{
    /// <summary>
    /// Метод конвертации параметра
    /// </summary>
    /// <param name="value">Параметр</param>
    /// <returns>Сконвертированный параметр</returns>
    T Convert(string value);
}