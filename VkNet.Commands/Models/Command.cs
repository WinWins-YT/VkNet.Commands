using VkNet.Commands.Abstractions;

namespace VkNet.Commands.Models;

/// <summary>
/// Параметры команды
/// </summary>
public class Command
{
    /// <summary>
    /// Полный текст команды
    /// </summary>
    public string FullCommand { get; set; } = "";

    /// <summary>
    /// Конвертеры параметров
    /// </summary>
    public Dictionary<Type, IParameterConverter> ParameterConverters { get; set; } = new();
}