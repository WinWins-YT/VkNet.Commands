using System.Reflection;
using FluentAssertions;
using Moq;
using VkNet.Abstractions;
using VkNet.Commands.Abstractions;
using VkNet.Commands.Attributes;
using VkNet.Commands.Converters;
using VkNet.Commands.Extensions;
using VkNet.Commands.Models;

namespace VkNet.Commands.Tests.CommandProcessor;

public class CommandProcessorTests
{
    [Fact]
    public void CommandProcessor_SearchCommandsIn_ShouldSuccess()
    {
        // Arrange
        var mock = new Mock<IVkApi>();
        var vkApi = mock.Object;
        var commandProcType = typeof(Commands.CommandProcessor);
        var commandDictionaryField = commandProcType.GetField("_commands", BindingFlags.Instance | BindingFlags.NonPublic);
        var testClassType = typeof(TestClass);
        var testMethod = testClassType.GetMethods().FirstOrDefault(x =>
        {
            var attib = x.GetCustomAttribute<CommandAttribute>();
            if (attib is null)
                return false;

            return attib.Name == "test";
        });
        var testAbcMethod = testClassType.GetMethods().FirstOrDefault(x =>
        {
            var attib = x.GetCustomAttribute<CommandAttribute>();
            if (attib is null)
                return false;

            return attib.Name == "test123";
        });
        
        var command = vkApi.AddCommands(new CommandsConfiguration
        {
            GroupId = 1
        });
        
        // Act
        command.SearchCommandsIn<TestClass>();
        
        // Assert
        var commandDictionary = (Dictionary<string, (Type, MethodInfo)>?)commandDictionaryField?.GetValue(command);

        commandDictionary.Should().NotBeNull();
        commandDictionary.Should().Contain(x =>
            x.Key == "test" && x.Value.Item1 == testClassType && x.Value.Item2 == testMethod);
        commandDictionary.Should().Contain(x =>
            x.Key == "test123" && x.Value.Item1 == testClassType && x.Value.Item2 == testAbcMethod);
    }

    [Fact]
    public void CommandProcessor_AddConverter_ShouldSuccess()
    {
        // Arrange
        var mock = new Mock<IVkApi>();
        var converterMock = new Mock<IParameterConverter<long>>();
        var vkApi = mock.Object;
        var commandProcType = typeof(Commands.CommandProcessor);
        var commandConvertersField = commandProcType.GetField("_parameterConverters", BindingFlags.Instance | BindingFlags.NonPublic);
        
        var command = vkApi.AddCommands(new CommandsConfiguration
        {
            GroupId = 1
        });
        
        // Act
        command.AddConverter(converterMock.Object);
        
        // Assert
        var converters = (Dictionary<Type, IParameterConverter>?)commandConvertersField?.GetValue(command);

        converters.Should().NotBeNull();
        converters.Should().Contain(x =>
            x.Key == typeof(long) && x.Value is IParameterConverter<long>);
        converters.Should().Contain(x =>
            x.Key == typeof(long?) && x.Value is IParameterConverter<long?> && x.Value is NullableConverter<long>);
    }
}