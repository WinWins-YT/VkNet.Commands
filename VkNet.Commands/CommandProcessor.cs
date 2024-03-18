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

public class CommandProcessor(CommandsConfiguration configuration)
{
    private readonly Dictionary<string, (Type, MethodInfo)> _commands = new();

    private readonly Dictionary<Type, IParameterConverter> _parameterConverters = new()
    {
        [typeof(string)] = new StringConverter(),
        [typeof(int)] = new Int32Converter()
    };

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

    public void AddConverter<T>(IParameterConverter<T> converter)
    {
        ArgumentNullException.ThrowIfNull(converter);

        _parameterConverters[typeof(T)] = converter;
    }

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

                    var commandArgs = CommandParser.GetCommandParameters(_commands[commandText].Item2, messageText,
                        message, configuration.VkApi);
                    if (commandArgs is null)
                        throw new InvalidMethodException("Command methods must have Message and IVkApi arguments");
                    
                    command.Item2.Invoke(Activator.CreateInstance(command.Item1), commandArgs);
                }
            }
        });

        return longPool.RunAsync(cancellationToken);
    }
}