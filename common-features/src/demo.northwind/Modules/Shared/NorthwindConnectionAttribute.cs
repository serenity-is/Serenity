namespace Serenity.Demo.Northwind;

/// <summary>
/// Specifies the connection key used by the Northwind feature.
/// </summary>
public class NorthwindConnectionAttribute() : ConnectionKeyAttribute(Key)
{
    /// <summary>
    /// Connection key used for the Northwind feature.
    /// </summary>
    public const string Key = "Northwind";
}
