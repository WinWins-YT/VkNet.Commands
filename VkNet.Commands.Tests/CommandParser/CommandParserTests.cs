using System.Reflection;
using FluentAssertions;
using Moq;
using VkNet.Abstractions;
using VkNet.Commands.Abstractions;
using VkNet.Commands.Converters;
using VkNet.Commands.Exceptions;
using VkNet.Commands.Models;
using VkNet.Model;

namespace VkNet.Commands.Tests.CommandParser;

public class CommandParserTests
{
    [Theory]
    [InlineData("123", 123)]
    [InlineData("43321", 43321)]
    public void CommandParser_ConvertInt32Argument_ShouldSuccess(string value, int expected)
    {
        // Arrange
        var parserType = typeof(Parsers.CommandParser);
        var convertArgumentMethod = parserType.GetTypeInfo().DeclaredMethods
            .First(x => x is
                { Name: "ConvertArgument", ContainsGenericParameters: false, IsStatic: true, IsPrivate: true });
        Parsers.CommandParser.Converters = new Dictionary<Type, IParameterConverter>
        {
            [typeof(int)] = new Int32Converter()
        };

        // Act
        var convertedValue = (int?)convertArgumentMethod.Invoke(null, [value, typeof(int), new Command()]);
        
        // Assert
        convertedValue.Should().NotBeNull();
        convertedValue.Should().Be(expected);
    }

    [Fact]
    public void CommandParser_ConvertTypeWithInvalidConverter_ShouldThrow()
    {
        // Arrange
        var parserType = typeof(Parsers.CommandParser);
        var convertArgumentMethod = parserType.GetTypeInfo().DeclaredMethods
            .First(x => x is
                { Name: "ConvertArgument", ContainsGenericParameters: false, IsStatic: true, IsPrivate: true });
        var converterMock = new Mock<IParameterConverter>();
        Parsers.CommandParser.Converters = new Dictionary<Type, IParameterConverter>
        {
            [typeof(long)] = converterMock.Object
        };
        
        // Act
        var method = () =>
        {
            try
            {
                convertArgumentMethod.Invoke(null, ["123", typeof(long), new Command()]);
            }
            catch (System.Exception ex) when (ex is TargetInvocationException or InvalidCastException)
            {
                ex.InnerException!.InnerException.Should().BeOfType<ArgumentException>();
                throw ex.InnerException!;
            }
        };
        
        // Assert
        method.Should().Throw<ParameterConverterException>();
    }

    [Theory]
    [InlineData("/test param1 param2 param3", new object?[] {"param1", "param2", "param3"})]
    [InlineData("/test param2 param3", new object?[] {"param2", "param3", null})]
    [InlineData("/test param1", new object?[] {"param1", null, null})]
    [InlineData("/test", new object?[] {null, null, null})]
    public void CommandParser_ParseCommandWithoutConverters_ShouldSuccess(string command, object?[] expected)
    {
        // Arrange
        var vkMock = new Mock<IVkApi>();
        var commandClassType = typeof(TestClass);
        var testMethod = commandClassType.GetMethod("TestMethod", BindingFlags.Public | BindingFlags.Instance);
        if (testMethod is null)
            Assert.Fail("Test method is null");
        
        // Act
        var parameters = Parsers.CommandParser.GetCommandParameters(testMethod, command, new Message(), vkMock.Object);
        
        // Assert
        parameters.Should().NotBeNull();
        parameters.Should().ContainInOrder(expected);
    }

    [Theory]
    [InlineData("/test param1 123 345", new object[] {"param1", 123, 345})]
    [InlineData("/test abc 654 321", new object[] {"abc", 654, 321})]
    [InlineData("/test abc 123", new object?[] {"abc", 123, null})]
    [InlineData("/test abc", new object?[] {"abc", null, null})]
    public void CommandParser_ParseCommandWithConverter_ShouldSuccess(string command, object?[] expected)
    {
        // Arrange
        var vkMock = new Mock<IVkApi>();
        var commandClassType = typeof(TestClass);
        var testMethod = commandClassType.GetMethod("TestMethodConverter", BindingFlags.Public | BindingFlags.Instance);
        if (testMethod is null)
            Assert.Fail("Test method is null");
        Parsers.CommandParser.Converters = new Dictionary<Type, IParameterConverter>
        {
            [typeof(int)] = new Int32Converter()
        };
        
        // Act
        var parameters = Parsers.CommandParser.GetCommandParameters(testMethod, command, new Message(), vkMock.Object);
        
        // Assert
        parameters.Should().NotBeNull();
        parameters.Should().ContainInOrder(expected);
    }

    [Theory]
    [InlineData("/test param1 param2 param3 param4 param5", new object?[] {"param1", "param2", "param3 param4 param5"})]
    [InlineData("/test param1 param2 param3", new object?[] {"param1", "param2", "param3"})]
    [InlineData("/test param2 param3", new object?[] {"param2", "param3", null})]
    [InlineData("/test param1", new object?[] {"param1", null, null})]
    [InlineData("/test", new object?[] {null, null, null})]
    public void CommandParser_ParseCommandWithRemainingAttribute_ShouldSuccess(string command, object?[] expected)
    {
        // Arrange
        var vkMock = new Mock<IVkApi>();
        var commandClassType = typeof(TestClass);
        var testMethod =
            commandClassType.GetMethod("TestMethodWithRemaining", BindingFlags.Public | BindingFlags.Instance);
        if (testMethod is null)
            Assert.Fail("Test method is null");
        
        // Act
        var parameters = Parsers.CommandParser.GetCommandParameters(testMethod, command, new Message(), vkMock.Object);
        
        // Assert
        parameters.Should().NotBeNull();
        parameters.Should().ContainInOrder(expected);
    }
    
    [Fact]
    public void CommandParser_ParseCommand_NoConverter_ShouldThrow()
    {
        // Arrange
        const string command = "/test abc 123 234";
        var vkMock = new Mock<IVkApi>();
        var commandClassType = typeof(TestClass);
        var testMethod = commandClassType.GetMethod("TestMethodConverter", BindingFlags.Public | BindingFlags.Instance);
        if (testMethod is null)
            Assert.Fail("Test method is null");
        Parsers.CommandParser.Converters = new Dictionary<Type, IParameterConverter>();
        
        // Act
        var method = () =>
        {
            var parameters = Parsers.CommandParser.GetCommandParameters(testMethod, command, new Message(), vkMock.Object);
        };
        
        // Assert
        method.Should().Throw<NoParameterConverterException>();
    }

    [Fact]
    public void CommandParser_ParseCommand_InvalidMethod_ShouldThrow()
    {
        // Arrange
        const string command = "/test abc 123 234";
        var vkMock = new Mock<IVkApi>();
        var commandClassType = typeof(TestClass);
        var testMethod = commandClassType.GetMethod("TestInvalidMethod", BindingFlags.Public | BindingFlags.Instance);
        if (testMethod is null)
            Assert.Fail("Test method is null");
        
        // Act
        var method = () =>
        {
            var parameters = Parsers.CommandParser.GetCommandParameters(testMethod, command, new Message(), vkMock.Object);
        };
        
        // Assert
        method.Should().Throw<InvalidMethodException>();
    }
}