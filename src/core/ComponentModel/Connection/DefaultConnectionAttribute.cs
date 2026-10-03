namespace Serenity.Data;

/// <summary>
/// Specifies the "Default" connection key.
/// </summary>
public class DefaultConnectionAttribute() : ConnectionKeyAttribute(Key)
{
    /// <summary>
    /// "Default" connection key.
    /// </summary>
    public const string Key = "Default";
}