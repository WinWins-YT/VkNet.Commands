namespace VkNet.Commands.Abstractions;

public interface IParameterConverter {}

public interface IParameterConverter<out T> : IParameterConverter
{
    T Convert(string value);
}