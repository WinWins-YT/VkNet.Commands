namespace VkNet.Commands.Attributes;

/// <summary>
/// Атрибут для валидации команды
/// </summary>
[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
public class CommandValidatorAttribute : Attribute
{
    
}