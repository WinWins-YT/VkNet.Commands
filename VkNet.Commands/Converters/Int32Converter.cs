using VkNet.Commands.Abstractions;

namespace VkNet.Commands.Converters;

/// <summary>
/// Конвертер числа
/// </summary>
public class Int32Converter : IParameterConverter<int>
{
    /// <summary>
    /// Конвертирование строки в число
    /// </summary>
    /// <param name="value">Строка</param>
    /// <returns>Число</returns>
    public int Convert(string value) => System.Convert.ToInt32(value);
}