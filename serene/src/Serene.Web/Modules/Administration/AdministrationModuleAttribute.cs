namespace Serene.Administration;

/// <summary>
/// Specifies the module key used by the Administration module.
/// </summary>
public class AdministrationModuleAttribute() : ModuleAttribute(Key)
{
    /// <summary>
    /// Module key used for the Administration module.
    /// </summary>
    public const string Key = "Administration";
}
