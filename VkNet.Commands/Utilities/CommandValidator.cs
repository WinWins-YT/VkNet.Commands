using System.Reflection;
using VkNet.Commands.Abstractions;
using VkNet.Commands.Attributes;
using VkNet.Commands.Exceptions;
using VkNet.Model;

namespace VkNet.Commands.Utilities;

internal class CommandValidator
{
    internal static Dictionary<Type, ICommandValidator> Validators { get; set; } = new();
    private static readonly MethodInfo? GenericValidateMethod;
    
    static CommandValidator()
    {
        var methods = typeof(CommandValidator).GetTypeInfo().DeclaredMethods;
        GenericValidateMethod = methods.FirstOrDefault(xm => xm is { Name: nameof(Validate), ContainsGenericParameters: true, IsStatic: true, IsPrivate: true });
    }
    
    public static bool Validate(ICommandValidator validator, Type type, object value, Message message, params object?[] parameters)
    {
        var validateMethod = GenericValidateMethod?.MakeGenericMethod(type);
        if (validateMethod is null)
            throw new ArgumentException("Could not get validator for specified type", nameof(type));

        try
        {
            return (bool)validateMethod.Invoke(null, [validator, value, message, parameters])!;
        }
        catch (System.Exception ex) when (ex is TargetInvocationException or InvalidCastException)
        {
            throw new CommandValidatorException($"Could not validate {type} type", ex.InnerException ?? ex);
        }
    }

    private static bool Validate<T>(ICommandValidator validator, T attribute, Message message, params object?[] parameters) 
        where T : CommandValidatorAttribute
    {
        if (validator is not ICommandValidator<T> validatorT)
        {
            throw new ArgumentException("Invalid validator registered for this type", nameof(T));
        }

        return validatorT.Validate(attribute, message, parameters);
    }
}