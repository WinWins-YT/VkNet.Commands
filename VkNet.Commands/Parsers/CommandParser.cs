using System.Reflection;
using VkNet.Abstractions;
using VkNet.Commands.Abstractions;
using VkNet.Commands.Exceptions;
using VkNet.Model;

namespace VkNet.Commands.Parsers;

internal static class CommandParser
{
    internal static Dictionary<Type, IParameterConverter> Converters { get; set; } = new();
    private static readonly MethodInfo? GenericConverter;

    static CommandParser()
    {
        var methods = typeof(CommandParser).GetTypeInfo().DeclaredMethods;
        GenericConverter = methods.FirstOrDefault(xm => xm is { Name: nameof(ConvertArgument), ContainsGenericParameters: true, IsStatic: true, IsPrivate: true });
    }
    
    /// <summary>
    /// Парсинг параметров команды
    /// </summary>
    /// <param name="methodInfo">Метод команды</param>
    /// <param name="command">Строка команды</param>
    /// <param name="message">Сообщение для передачи в метод</param>
    /// <param name="vkApi">VkApi для передачи в метод</param>
    /// <returns>Массив параметров метода</returns>
    /// <exception cref="NoParameterConverterException">Если на тип в методе нет конвертора</exception>
    internal static object[]? GetCommandParameters(MethodInfo methodInfo, string command, Message message, IVkApi vkApi)
    {
        var parameters = methodInfo.GetParameters();
        var parameterTypes = parameters.Select(x => x.ParameterType).ToArray();

        if (!parameterTypes.Contains(typeof(Message)) || !parameterTypes.Contains(typeof(IVkApi)))
            return null;
        
        List<object> paramList = new();
        var commandArgs = command.Split(' ')[1..];
        var commandArgIndex = 0;

        foreach (var parameter in parameters)
        {
            var parameterType = parameter.ParameterType;

            if (parameterType == typeof(Message))
            {
                paramList.Add(message);
                continue;
            }

            if (parameterType == typeof(IVkApi))
            {
                paramList.Add(vkApi);
                continue;
            }
            
            if (!Converters.ContainsKey(parameterType))
                throw new NoParameterConverterException(
                    $"No parameter converter given for {parameter.ParameterType} type");

            var commandArg = commandArgs[commandArgIndex++];
            var param = ConvertArgument(commandArg, parameterType);
            paramList.Add(param);
        }

        return paramList.ToArray();
    }

    private static object ConvertArgument<T>(string value)
    {
        var t = typeof(T);
        
        if (Converters[t] is not IParameterConverter<T> converter)
        {
            throw new ArgumentException("Invalid converter registered for this type", nameof(T));
        }

        var converted = converter.Convert(value);
        return converted ??
               throw new ArgumentException("Could not convert specified value to given type", nameof(value));
    }

    /// <summary>
    /// Конвертация аргумента
    /// </summary>
    /// <param name="value">Аргумент</param>
    /// <param name="type">Тип, в который надо преобразовать</param>
    /// <returns>Преобразованный аргумент</returns>
    /// <exception cref="ArgumentException">Если не удалось сконвертировать</exception>
    /// <exception cref="Exception">Если не удалось сконвертировать</exception>
    private static object ConvertArgument(string value, Type type)
    {
        var convertMethod = GenericConverter?.MakeGenericMethod(type);
        if (convertMethod is null)
            throw new ArgumentException("Could not convert specified value to given type", nameof(value));
        
        try
        {
            return convertMethod.Invoke(null, [value]) ??
                   throw new ArgumentException("Could not convert specified value to given type", nameof(value));
        }
        catch (System.Exception ex) when (ex is TargetInvocationException or InvalidCastException)
        {
            throw ex.InnerException ?? ex;
        }
    }
}