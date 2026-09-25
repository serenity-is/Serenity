namespace Serenity.Demo.Northwind;

/// <summary>
/// Specifies the module key used by the Northwind feature.
/// </summary>
public class NorthwindModuleAttribute() : ModuleAttribute(Key)
{
    /// <summary>
    /// Module key used for the Northwind feature.
    /// </summary>
    public const string Key = "Northwind";
}
