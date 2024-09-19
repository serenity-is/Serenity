namespace Serenity.Web;

/// <summary>
/// Dynamic script that contains remote data
/// </summary>
public class DataScript : DynamicScript, INamedDynamicScript, IGetScriptData
{
    /// <summary>
    /// Key for the data script
    /// </summary>
    protected string key;

    /// <summary>
    /// Callback to get data
    /// </summary>
    protected Func<object> getData;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    protected DataScript()
    {
    }

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="key">Data script key</param>
    /// <param name="getData">Get data callback</param>
    /// <exception cref="ArgumentNullException"></exception>
    public DataScript(string key, Func<object> getData)
    {
        this.getData = getData ?? throw new ArgumentNullException(nameof(getData));
        this.key = key;
    }

    /// <inheritdoc/>
    public string ScriptName => "RemoteData." + key;

    /// <inheritdoc/>
    public object GetScriptData()
    {
        return getData();
    }

    /// <inheritdoc/>
    public override string GetScript()
    {
        var data = getData();
        return string.Format(CultureInfo.CurrentCulture,
            SetScriptDataFormat,
            ScriptName.ToSingleQuoted(),
            JSON.Stringify(data, writeNulls: false));
    }

    /// <summary>
    /// Format string for Serenity.setScriptData({0}, {1})
    /// </summary>
    public const string SetScriptDataFormat =
        "((typeof Serenity!=='undefined'&&Serenity.setScriptData)||" +
        "(function(k,v){{" +
            "var s=Symbol.for('Serenity.scriptData');" +
            "var g=globalThis[s];" +
            "if(!g)g=globalThis[s]={{}};" +
            "g[k]=v" +
        "}})" +
            ")({0},{1});";
}