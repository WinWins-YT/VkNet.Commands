using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using VkNet.Abstractions;
using VkNet.Commands.Attributes;

namespace VkNet.Commands.Tests.CommandProcessor;

public class TestClass
{
    [Command("test")]
    public Task TestCommand(string text, Message message, IVkApi vkApi)
    {
        return Task.CompletedTask;
    }

    [Command("test123")]
    public Task TestAbc(Message message, IVkApi vkApi)
    {
        return Task.CompletedTask;
    }

    public Task CommandWithoutAttribute(Message message, IVkApi vkApi)
    {
        return Task.CompletedTask;
    }
}