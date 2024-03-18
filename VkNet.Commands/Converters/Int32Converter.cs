using VkNet.Commands.Abstractions;

namespace VkNet.Commands.Converters;

public class Int32Converter : IParameterConverter<int>
{
    public int Convert(string value) => System.Convert.ToInt32(value);
}