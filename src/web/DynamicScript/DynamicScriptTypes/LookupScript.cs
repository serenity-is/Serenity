using System.Collections;

namespace Serenity.Web;

/// <summary>
/// Dynamic script type for lookup scripts.
/// </summary>
public abstract class LookupScript : DynamicScript, INamedDynamicScript, IGetScriptData
{
    private readonly Dictionary<string, object> lookupParams;

    /// <summary>
    /// Data format for a lookup script.
    /// </summary>
    /// <param name="Items">The item list.</param>
    /// <param name="Params">The lookup parameters.</param>
    public record Data(IEnumerable Items, Dictionary<string, object> Params);

    /// <summary>
    /// Initializes a new instance of the <see cref="LookupScript"/> class.
    /// </summary>
    protected LookupScript()
    {
        lookupParams = [];
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

        return string.Format(CultureInfo.InvariantCulture, DataScript.SetScriptDataFormat,
            ("Lookup." + LookupKey).ToSingleQuoted(), 
            string.Format(NewLookupFormat,
                JSON.Stringify(LookupParams, writeNulls: false), 
                JSON.Stringify(items, writeNulls: false)));
    }

    /// <summary>
    /// Format string for <c>new Lookup({0}, {1})</c>.
    /// </summary>
    public const string NewLookupFormat =
        "new ((typeof Serenity!=='undefined'&&Serenity.Lookup)||" +
        "(class Lookup{{" + 
            "constructor(o,a){{" + 
                "o&&Object.assign(this,o);a&&this.update(a)" + 
            "}}" + 
            "update(a){{" + 
                "var f=this.idField,v;this.itemById={{}};this.items=a||[];" + 
                "this.items.forEach(r=>{{if((v=r[f])!=null)this.itemById[v] = r}})" + 
            "}}" +
        "}}))({0},{1})";

    /// <summary>
    /// Gets the lookup parameters dictionary.
    /// </summary>
    public Dictionary<string, object> LookupParams => lookupParams;

    /// <summary>
    /// Gets or sets the lookup ID field.
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
    /// Gets or sets the lookup text field.
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
    /// Gets or sets the lookup parent ID field.
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
    /// Gets or sets the lookup key.
    /// </summary>
    public string LookupKey { get; set; }

    /// <inheritdoc/>
    public string ScriptName => "Lookup." + LookupKey;
}