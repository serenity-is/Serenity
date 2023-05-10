namespace Serenity.Web;

/// <summary>
/// A dynamic script that is formed from concentanation of other scripts
/// </summary>
public class ConcatenatedScript : DynamicScript
{
    private readonly string separator;
    private readonly IEnumerable<Func<string>> scriptParts;
    private readonly Action<IPermissionService, ITextLocalizer> checkRights;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="scriptParts">Script parts</param>
    /// <param name="separator">Separator</param>
    /// <param name="checkRights">Callback to check permissions for individual
    /// scripts</param>
    /// <exception cref="ArgumentNullException">Script parts is null</exception>
    public ConcatenatedScript(IEnumerable<Func<string>> scriptParts,
        string separator = "\r\n;\r\n", Action<IPermissionService, ITextLocalizer> checkRights = null)
    {
        this.scriptParts = scriptParts ?? throw new ArgumentNullException(nameof(scriptParts));
        this.separator = separator;
        this.checkRights = checkRights;
    }

    /// <inheritdoc/>
    public override void CheckRights(IPermissionService permissions, ITextLocalizer localizer)
    {
        base.CheckRights(permissions, localizer);

        checkRights?.Invoke(permissions, localizer);
    }

    /// <inheritdoc/>
    public override string GetScript()
    {
        StringBuilder sb = new();

        foreach (var part in scriptParts)
        {
            string partSource = part();

            sb.AppendLine(partSource);
            if (!string.IsNullOrEmpty(separator))
                sb.AppendLine(separator);
        }

        return sb.ToString();
    }
}