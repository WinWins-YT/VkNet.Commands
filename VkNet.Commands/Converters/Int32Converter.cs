using VkNet.Commands.Abstractions;
using VkNet.Commands.Models;

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
    /// <param name="command">Параметры команды</param>
    /// <returns>Число</returns>
    public int Convert(string value, Command command) => System.Convert.ToInt32(value);
}