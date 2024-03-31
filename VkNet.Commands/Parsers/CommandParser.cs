using System.Reflection;
using VkNet.Abstractions;
using VkNet.Commands.Abstractions;
using VkNet.Commands.Attributes;
using VkNet.Commands.Exceptions;
using VkNet.Commands.Models;
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
    internal static object?[] GetCommandParameters(MethodInfo methodInfo, string command, Message message, IVkApi vkApi)
    {
        var parameters = methodInfo.GetParameters();
        var parameterTypes = parameters.Select(x => x.ParameterType).ToArray();
        var commandParams = new Command
        {
            FullCommand = command,
            ParameterConverters = Converters
        };

        if (!parameterTypes.Contains(typeof(Message)) || !parameterTypes.Contains(typeof(IVkApi)))
            throw new InvalidMethodException("Command methods must have Message and IVkApi arguments");
        
        List<object?> paramList = [];
        var commandArgs = command.Split(' ')[1..];
        var commandArgIndex = 0;

        foreach (var parameter in parameters)
        {
            var parameterType = parameter.ParameterType;
            var remainingAttribute = parameter.GetCustomAttribute<RemainingTextAttribute>();
            //var optionalAttribute = parameter.GetCustomAttribute<OptionalParameterAttribute>();

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

            if (remainingAttribute is not null && parameterType == typeof(string))
            {
                var remaining = string.Join(' ', commandArgs[commandArgIndex..]);
                paramList.Add(remaining == string.Empty ? null : remaining);
                continue;
            }
            
            if (!Converters.ContainsKey(parameterType) && parameterType != typeof(string))
                throw new NoParameterConverterException(
                    $"No parameter converter given for {parameter.ParameterType} type");

            if (commandArgs.Length == commandArgIndex)
            {
                paramList.Add(null);
                continue;
            }
            
            var commandArg = commandArgs[commandArgIndex++];
            if (parameterType == typeof(string))
            {
                paramList.Add(commandArg);
                continue;
            }
            
            var param = ConvertArgument(commandArg, parameterType, commandParams);
            paramList.Add(param);
        }

        while (paramList.Count < parameters.Length)
        {
            paramList.Add(null);
        }

        return paramList.ToArray();
    }

    private static object ConvertArgument<T>(string value, Command command)
    {
        var t = typeof(T);
        
        if (Converters[t] is not IParameterConverter<T> converter)
        {
            throw new ArgumentException("Invalid converter registered for this type", nameof(T));
        }

        var converted = converter.Convert(value, command);
        return converted ??
               throw new ArgumentException("Could not convert specified value to given type", nameof(value));
    }

    /// <summary>
    /// Конвертация аргумента
    /// </summary>
    /// <param name="value">Аргумент</param>
    /// <param name="type">Тип, в который надо преобразовать</param>
    /// <param name="command">Параметры команды</param>
    /// <returns>Преобразованный аргумент</returns>
    /// <exception cref="ArgumentException">Если не удалось сконвертировать</exception>
    /// <exception cref="Exception">Если не удалось сконвертировать</exception>
    private static object ConvertArgument(string value, Type type, Command command)
    {
        var convertMethod = GenericConverter?.MakeGenericMethod(type);
        if (convertMethod is null)
            throw new ArgumentException("Could not convert specified value to given type", nameof(value));
        
        try
        {
            return convertMethod.Invoke(null, [value, command]) ??
                   throw new ArgumentException("Could not convert specified value to given type", nameof(value));
        }
        catch (System.Exception ex) when (ex is TargetInvocationException or InvalidCastException)
        {
            throw new ParameterConverterException($"Could not convert {value} to {type} type", ex.InnerException ?? ex);
        }
    }
}