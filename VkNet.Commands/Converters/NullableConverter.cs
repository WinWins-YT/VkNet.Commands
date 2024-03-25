using VkNet.Commands.Abstractions;
using VkNet.Commands.Exceptions;
using VkNet.Commands.Models;

namespace VkNet.Commands.Converters;

/// <summary>
/// Конвертер Nullable типов
/// </summary>
/// <typeparam name="T">Nullable тип</typeparam>
public class NullableConverter<T> : IParameterConverter<T?> where T: struct
{
    /// <summary>
    /// Конвертирует Nullable тип в не Nullable и использует обычный конвертер
    /// </summary>
    /// <param name="value">Строка</param>
    /// <param name="command">Параметры команды</param>
    /// <returns>Сконвертированный тип</returns>
    /// <exception cref="NoParameterConverterException">Если нет конвертера необходимого типа</exception>
    public T? Convert(string value, Command command)
    {
        if (command.ParameterConverters.TryGetValue(typeof(T), out var converter))
        {
            var typeConverter = (IParameterConverter<T>)converter;
            var converted = typeConverter.Convert(value, command);
            return converted;
        }

        throw new NoParameterConverterException($"No parameter converter given for {typeof(T)} type");
    }
}