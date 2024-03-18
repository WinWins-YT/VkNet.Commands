using VkNet.Commands.Abstractions;

namespace VkNet.Commands.Converters;

public class StringConverter : IParameterConverter<string>
{
    public string Convert(string value) => value;
}