namespace Serenity.Web;

/// <summary>
/// Generic subclass of the <see cref="DataScript"/>
/// </summary>
/// <typeparam name="TData">Data type</typeparam>
public abstract class DataScript<TData> : DataScript
    where TData: class
{
    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    protected DataScript()
    {
        getData = GetData;
        var attr = GetType().GetCustomAttribute<DataScriptAttribute>();
        if (attr != null)
        {
            key = attr.Key;
            if (attr.Key == null)
                key = DataScriptAttribute.AutoKeyFor(GetType());
                
            Expiration = TimeSpan.FromSeconds(attr.CacheDuration);
            Permission = attr.Permission;
            GroupKey = attr.CacheGroupKey;
        }
    }

    /// <summary>
    /// Gets the data object
    /// </summary>
    protected abstract TData GetData();
}