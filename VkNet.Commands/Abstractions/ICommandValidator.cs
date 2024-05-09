using VkNet.Commands.Attributes;
using VkNet.Model;

namespace VkNet.Commands.Abstractions;

/// <summary>
/// Интерфейс для валидатора команд
/// </summary>
public interface ICommandValidator {}


/// <summary>
/// Интерфейс для валидатора команд
/// </summary>
/// <typeparam name="T">Атрибут валидатора</typeparam>
public interface ICommandValidator<in T> : ICommandValidator where T : CommandValidatorAttribute
{
    /// <summary>
    /// Вызывается при наличии атрибута валидации у команды
    /// </summary>
    /// <param name="attribute">Атрибут валидатора</param>
    /// <param name="message">Сообщение</param>
    /// <param name="parameters">Параметры, спарсенные из сообщения</param>
    /// <returns><para>true - если валидация успешная</para><para>false - в противном случае, будет вызвано исключение, которое можно перехватить из события исключения</para></returns>
    public bool Validate(T attribute, Message message, params object?[] parameters);
}