using VkNet.Commands.Abstractions;

namespace VkNet.Commands.Converters;

/// <summary>
/// Конвертер строки
/// </summary>
public class StringConverter : IParameterConverter<string>
{
    /// <summary>
    /// Конвертирование строки в строку :)
    /// </summary>
    /// <param name="value">Строка</param>
    /// <returns>Строка</returns>
    public string Convert(string value) => value;
}