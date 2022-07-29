namespace Serenity.Web
{
    public class DataScript : DynamicScript, INamedDynamicScript
    {
        protected string key;
        protected Func<object> getData;

        protected DataScript()
        {
        }

        public DataScript(string key, Func<object> getData)
        {
            this.getData = getData ?? throw new ArgumentNullException(nameof(getData));
            this.key = key;
        }

        public string ScriptName => "RemoteData." + key;

        public override string GetScript(DynamicScriptResponseType responseType)
        {
            var data = getData();

            switch (responseType)
            {
                case DynamicScriptResponseType.Json:
                    return data.ToJson();
                case DynamicScriptResponseType.JavaScript:
                case DynamicScriptResponseType.Default:
                default:
                    return string.Format(CultureInfo.CurrentCulture, "Q.ScriptData.set({0}, {1});", ScriptName.ToSingleQuoted(), data.ToJson());
            }
        }
    }

    public abstract class DataScript<TData> : DataScript
        where TData: class
    {
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

        protected abstract TData GetData();
    }
}