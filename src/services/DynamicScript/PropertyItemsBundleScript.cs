namespace Serenity.Web;

/// <summary>
/// A bundle of form and/or columns scripts in compact format, which only contains
/// the scripts that the current user has permission for. A separate content is
/// cached for each combination of accessible scripts via the cache suffix.
/// </summary>
public class PropertyItemsBundleScript : DynamicScript, ICacheSuffix
{
    private readonly PropertyItemsScript[] scripts;
    private readonly PropertyItemsScript[] checkedScripts;
    private readonly IPermissionService? permissions;

    /// <summary>
    /// Creates a new instance of the class.
    /// </summary>
    /// <param name="scripts">Form and/or columns scripts</param>
    /// <param name="permissions">Permission service. If null, scripts that
    /// require a permission are never included.</param>
    public PropertyItemsBundleScript(IEnumerable<PropertyItemsScript> scripts, IPermissionService? permissions)
    {
        this.scripts = [.. scripts ?? throw new ArgumentNullException(nameof(scripts))];
        this.permissions = permissions;
        checkedScripts = [.. this.scripts.Where(x => x.Permission is not null)];
        Permission = SpecialPermissionKeys.Public;
    }

    private bool IsIncluded(PropertyItemsScript script)
    {
        return script.Permission is null ||
            (permissions is not null && script.HasPermission(permissions));
    }

    /// <summary>
    /// Gets a suffix identifying which permission checked scripts are accessible
    /// for the current user. Empty if no script requires a permission.
    /// </summary>
    public string CacheSuffix
    {
        get
        {
            if (checkedScripts.Length == 0)
                return "";

            var bits = new byte[(checkedScripts.Length + 7) / 8];
            for (var i = 0; i < checkedScripts.Length; i++)
                if (IsIncluded(checkedScripts[i]))
                    bits[i / 8] |= (byte)(1 << (i % 8));

            return Convert.ToBase64String(bits);
        }
    }

    /// <inheritdoc/>
    public override string GetScript()
    {
        return PropertyItemsScript.Compact(scripts
            .Where(IsIncluded)
            .Select(x => (x.ScriptName, (PropertyItemsData)x.GetScriptData())));
    }
}
