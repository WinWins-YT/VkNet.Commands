using VkNet.Abstractions;
using VkNet.Commands.Attributes;
using VkNet.Model;

namespace VkNet.Commands.Tests.CommandParser;

public class TestClass
{
    [Command("test")]
    public void TestMethod(string param1, string param2, string param3, Message message, IVkApi vkApi)
    {
        
    }

    [Command("test")]
    public void TestMethodConverter(string param1, int param2, int param3, Message message, IVkApi vkApi)
    {
        
    }

    public void TestMethodWithRemaining(string param1, string param2, [RemainingText] string param3, Message message,
        IVkApi vkApi)
    {
        
    }

    [Command("test")]
    public void TestInvalidMethod(string param1, string param2, string param3)
    {
        
    }
}