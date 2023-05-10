using System.Collections;

namespace Serenity.Web;

/// <summary>
/// Dynamic script type for lookup scripts
/// </summary>
public abstract class LookupScript : DynamicScript, INamedDynamicScript, IGetScriptData
{
    private readonly Dictionary<string, object> lookupParams;

    /// <summary>
    /// Data format for a lookup script
    /// </summary>
    /// <param name="Items">Item list</param>
    /// <param name="Params">Lookup params</param>
    public record Data(IEnumerable Items, Dictionary<string, object> Params);

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    protected LookupScript()
    {
        lookupParams = new Dictionary<string, object>();
    }

    /// <inheritdoc/>
    protected abstract IEnumerable GetItems();

    /// <inheritdoc/>
    public object GetScriptData()
    {
        return new Data(GetItems(), LookupParams);
    }

    /// <inheritdoc/>
    public override string GetScript()
    {
        IEnumerable items = GetItems();

        return string.Format(CultureInfo.InvariantCulture, "Q.ScriptData.set({0}, new Q.Lookup({1}, \n{2}\n));",
            ("Lookup." + LookupKey).ToSingleQuoted(), LookupParams.ToJson(), items.ToJson());
    }

    /// <summary>
    /// Gets lookup parameters dictionary
    /// </summary>
    public Dictionary<string, object> LookupParams => lookupParams;

    /// <summary>
    /// Gets / sets lookup ID field
    /// </summary>
    public string IdField
    {
        get
        {
            if (lookupParams.TryGetValue("idField", out object value) && value != null)
                return value.ToString();

            return null;
        }
        set
        {
            lookupParams["idField"] = value;
        }
    }

    /// <summary>
    /// Gets / sets lookup text field
    /// </summary>
    public string TextField
    {
        get
        {
            if (lookupParams.TryGetValue("textField", out object value) && value != null)
                return value.ToString();

            return null;
        }
        set
        {
            lookupParams["textField"] = value;
        }
    }

    /// <summary>
    /// Gets / sets lookup parent ID field
    /// </summary>

    public string ParentIdField
    {
        get
        {
            if (lookupParams.TryGetValue("parentIdField", out object value) && value != null)
                return value.ToString();

            return null;
        }
        set
        {
            lookupParams["parentIdField"] = value;
        }
    }

    /// <summary>
    /// Gets / sets lookup key
    /// </summary>
    public string LookupKey { get; set; }

    /// <inheritdoc/>
    public string ScriptName => "Lookup." + LookupKey;
}