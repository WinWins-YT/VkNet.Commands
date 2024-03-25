namespace VkNet.Commands.Attributes;

/// <summary>
/// Атрибут для обозначения параметра в который будет переданы оставшиеся команды
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class RemainingTextAttribute : Attribute;