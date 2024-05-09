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

### Исключения команд

Для отслеживания исключений команд, есть событие CommandException

```C#
commands.CommandException += (s, e)
{
    if (e.CommandException is CommandException cex)
    {
        // Исключение вызвано командой
        
        // e.FullCommandText - полный текст команды
        // cex.InnerException - исключение, которое было брошено командой
        // cex.CommandClass - объект класса команды
    }
    
    if (e.CommandException is ParameterConverterException pex)
    {
        // Исключение вызвано конвертером команд
        
        // e.FullCommandText - полный текст команды
        // pex.InnerException - исключение, которое было брошено конвертером параметров
    }
}
```

### Валидаторы команд

Команды могут проходить проверку перед запуском, например на вызов от администратора

Валидатор состоит из атрибута, который наследуется от `CommandValidatorAttribute` и может содержать некоторые параметры.

Вторая часть это сам валидатор, который наследуется от `ICommandValidator<T>`, где `T` - тип атрибута, наследованного от `CommandValidatorAttribute`

Функция `Validate` должна возвращать `true` при удачной валидации и `false` при неудачной

Пример валидатора, который проверяет год запуска:

```C#
public class RequireYearValidatorAttribute(int year) : CommandValidatorAttribute
{
    public int Year { get; set; } = year;
}

public class RequireYearValidator : ICommandValidator<RequireYearValidatorAttribute>
{
    public bool Validate(RequireYearValidatorAttribute attribute, Message message, params object?[] parameters)
    { 
        return DateTime.Now.Year > attribute.Year;
    }
}
```

Затем его необходимо добавить в процессор команд:

```C#
commands.AddValidator(new RequireYearValidator());
```

Применение к команде `square`:

```C#
[Command("square"), RequireYearValidator(2025)]
public async Task Square(int? number, Message message, IVkApi vkApi)
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

В данном случае будет проведена проверка, что год запуска больше чем 2025

В случае неудачной валидации будет вызвано исключение `FailedValidationException`, которое можно обработать в `CommandErrored` событии

```C#
commands.CommandException += (s, e) =>
{
    if (e.CommandException is FailedValidationException validationException)
    {
        if (validationException.Validator is RequireYearValidatorAttribute)
        {
            vk.Messages.Send(new MessagesSendParams
            {
                RandomId = new Random().Next(),
                PeerId = validationException.MessageObject.UserId,
                Message = "Вы не можете использовать эту команду в настоящее время"
            });
        }
    }
};
```