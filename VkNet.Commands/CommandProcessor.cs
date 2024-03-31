using System.Reflection;
using VkNet.Commands.Abstractions;
using VkNet.Commands.Attributes;
using VkNet.Commands.Converters;
using VkNet.Commands.Exceptions;
using VkNet.Commands.Extensions;
using VkNet.Commands.Models;
using VkNet.Commands.Parsers;
using VkNet.Enums.StringEnums;
using VkNet.Model;
using VkNet.Utils.BotsLongPoll;

namespace VkNet.Commands;

/// <summary>
/// Процессор команд
/// </summary>
/// <param name="configuration">Конфигурация процессора команд</param>
public class CommandProcessor(CommandsConfiguration configuration)
{
    private readonly Dictionary<string, (Type, MethodInfo)> _commands = new();

    private readonly Dictionary<Type, IParameterConverter> _parameterConverters = new()
    {
        [typeof(int)] = new Int32Converter(),
        [typeof(int?)] = new NullableConverter<int>()
    };
    
    /// <summary>
    /// Добавляет класс в котором находятся методы команд
    /// </summary>
    /// <typeparam name="T">Тип класса с методами команд</typeparam>
    public void SearchCommandsIn<T>() where T : class
    {
        var type = typeof(T);

        foreach (var method in type.GetMethods())
        {
            var commandAttribute = method.GetCustomAttribute<CommandAttribute>(false);
            if (commandAttribute is null)
                continue;
            
            _commands.Add(commandAttribute.Name, (type, method));
        }
    }

    /// <summary>
    /// Добавляет конвертор параметров
    /// </summary>
    /// <param name="converter">Конвертор параметров</param>
    /// <typeparam name="T">Тип, к которому приводит конвертор</typeparam>
    public void AddConverter<T>(IParameterConverter<T> converter)
    {
        ArgumentNullException.ThrowIfNull(converter);
        var type = typeof(T);
        _parameterConverters[type] = converter;
        
        var nullableConverterType = typeof(NullableConverter<>).MakeGenericType(type);
        var nullableType = typeof(Nullable<>).MakeGenericType(type);
        
        if (_parameterConverters.ContainsKey(nullableType))
        {
            return;
        }

        if (Activator.CreateInstance(nullableConverterType) is IParameterConverter nullableConverter)
        {
            _parameterConverters[nullableType] = nullableConverter;
        }
    }

    /// <summary>
    /// Запуск отслеживания сообщений и выполнения команд
    /// </summary>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Если VkApi null</exception>
    /// <exception cref="InvalidMethodException">Если метод команды неверный</exception>
    public Task StartListening(CancellationToken cancellationToken = default)
    {
        if (configuration.VkApi is null)
            throw new ArgumentNullException(nameof(configuration.VkApi), "VkApi cannot be null");

        CommandParser.Converters = _parameterConverters;

        var longPool = new BotsLongPollUpdatesHandler(new BotsLongPollUpdatesHandlerParams(configuration.VkApi, configuration.GroupId)
        {
            GetPause = () => false,
            
            OnException = Console.WriteLine,
            
            OnUpdates = e =>
            {
                var updates = new List<GroupUpdate>();

                foreach (var update in e.Updates)
                {
                    if (update.Update != null)
                    {
                        updates.Add(update.Update);
                    }
                }

                if (updates.Count == 0)
                    return;

                var newMessages = updates.Where(x => x.Type.Value == GroupUpdateType.MessageNew)
                    .Select(x => x.Instance as Message);

                foreach (var message in newMessages)
                {
                    if (message is null)
                        continue;

                    var messageText = message.Text ?? message.Body;
                    
                    var commandText = messageText.Split(' ')[0];
                    if (!configuration.Prefixes.Any(x => messageText.StartsWith(x))) 
                        continue;
                    
                    commandText = commandText.RemovePrefixes(configuration.Prefixes);
                    if (!_commands.ContainsKey(commandText)) 
                        continue;
                    
                    var command = _commands[commandText];
                    var commandClass = Activator.CreateInstance(command.Item1);

                    try
                    {
                        var commandArgs = CommandParser.GetCommandParameters(_commands[commandText].Item2, messageText,
                            message, configuration.VkApi);

                        command.Item2.Invoke(commandClass, commandArgs);
                    }
                    catch (TargetInvocationException ex)
                    {
                        var commandExceptionArgs = new CommandExceptionArgs
                        {
                            FullCommandText = messageText,
                            CommandException = new CommandException("Command execution caused an exception", ex.InnerException!, commandClass)
                        };
                        CommandException?.Invoke(this, commandExceptionArgs);
                    }
                    catch (System.Exception ex)
                    {
                        var commandExceptionArgs = new CommandExceptionArgs
                        {
                            FullCommandText = messageText,
                            CommandException = ex
                        };
                        CommandException?.Invoke(this, commandExceptionArgs);
                    }
                }
            }
        });

        return longPool.RunAsync(cancellationToken);
    }

    /// <summary>
    /// Хендлер исключения команды
    /// </summary>
    public delegate void CommandExceptionEventHandler(object? sender, CommandExceptionArgs e);

    /// <summary>
    /// Событие, когда исполняемая команда или конвертер параметров вернул исключение
    /// </summary>
    public event CommandExceptionEventHandler? CommandException;
}