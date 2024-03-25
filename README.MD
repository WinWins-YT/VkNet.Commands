# VkNet.Commands

Расширение для [VkNet](https://github.com/vknet/vk) для создания методов команд бота

## Подключение

### Добавление Commands в VkApi
```C#
using VkNet.Commands.Extensions;

var commands = vkApi.AddCommands(new CommandsConfiguration
{
    Prefixes = ["!", "/"],
    GroupId = 123
});
```
`Prefixes` - префиксы для команды. Например для команды test при префиксах `/` и `!` валидными будут `/test` и `!test`

`GroupId` - Id ВК группы

### Добавление класса с командами в CommandProcessor

```C#
commands.SearchCommandsIn<T>();
```
`T` - класс с командами. Например Commands - `commands.SearchCommandsIn<Commands>();`

### Запуск

```C#
await commands.StartListening();
```

## Создание команд

Команды располагаются в классе и отмечаются аттрибутом `Command` с параметром `name`

Метод обязан принимать в себя `Message` и `IVkApi`
```C#
[Command("test")]
public async Task TestMessage(Message message, IVkApi vkApi)
{
    Console.WriteLine("Test");
    await vkApi.Messages.SendAsync(new MessagesSendParams
    {
        PeerId = message.PeerId ?? message.UserId,
        Message = "Ping pong",
        RandomId = Random.Shared.Next()
    });
}
```

### Команды с параметрами

Команды также могут принимать в себя параметры разделенные пробелом, которые берутся из сообщения

Пример:
```C#
[Command("square")]
public async Task Square(int number, Message message, IVkApi vkApi)
{
    var userId = message.UserId ?? 0;
    
    await vkApi.Messages.SendAsync(new MessagesSendParams
    {
        PeerId = userId,
        Message = $"Квадрат числа {number} = {number * number}",
        RandomId = Random.Shared.Next()
    });
}
```

Команда square с параметром `int number` принимает число, например `/square 5`

Если параметра в сообщении нет, то в метод будет передано null

Для получения параметра, состоящего из нескольких слов, используется атрибут `RemainingText`

Пример:
```C#
[Command("sayhello")]
public async Task SayHello(string name, [RemainingText] string text, Message message, IVkApi vkApi)
{
    var userId = message.UserId ?? 0;

    await vkApi.Messages.SendAsync(new MessagesSendParams
    {
        PeerId = userId,
        Message = $"Saying hello to {name} with message: {text}",
        RandomId = Random.Shared.Next()
    });
}
```
`/sayhello Nick Let's play some games` в метод будет передано: `name` = Nick, `text` = Let's play some games

### Конвертеры параметров

Пользователь может самостоятельно писать конвертеры параметров для типов, которые библиотека не может сконвертировать

Для этого нужно создать класс, который наследуется от `IParameterConverter<T>`

```C#
public class Int32Converter : IParameterConverter<int>
{
    public int Convert(string value, Command command) => System.Convert.ToInt32(value);
}
```

И добавить его в commands

```C#
commands.AddConverter(new Int32Converter());
```

Теперь данный тип можно использовать как аргумент метода команды