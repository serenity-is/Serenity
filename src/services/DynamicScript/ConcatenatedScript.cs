namespace Serenity.Web;

/// <summary>
/// A dynamic script that is formed from the concatenation of other scripts.
/// </summary>
/// <remarks>
/// Creates a new instance of the class.
/// </remarks>
/// <param name="scriptParts">The script parts.</param>
/// <param name="separator">The separator.</param>
/// <param name="checkRights">Callback to check permissions for individual
/// scripts.</param>
/// <exception cref="ArgumentNullException">Script parts are null.</exception>
public class ConcatenatedScript(IEnumerable<Func<string>> scriptParts,
    string separator = "\r\n;\r\n", Action<IPermissionService, ITextLocalizer> checkRights = null) : DynamicScript
{
    private readonly string separator = separator;
    private readonly IEnumerable<Func<string>> scriptParts = scriptParts ?? throw new ArgumentNullException(nameof(scriptParts));
    private readonly Action<IPermissionService, ITextLocalizer> checkRights = checkRights;

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
            if (sb.Length > 0 && !string.IsNullOrEmpty(separator))
                sb.AppendLine(separator);
            sb.AppendLine(partSource);
        }

        return sb.ToString();
    }
}