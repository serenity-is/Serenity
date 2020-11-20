using Serenity.ComponentModel;
using System;
using System.Reflection;

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

        public string ScriptName { get { return "RemoteData." + key; } }

        public override string GetScript()
        {
            var data = getData();
            return String.Format("Q.ScriptData.set({0}, {1});", this.ScriptName.ToSingleQuoted(), data.ToJson());
        }
      
    }

    public abstract class DataScript<TData> : DataScript
        where TData: class
    {
        public DataScript()
        {
            this.getData = GetData;
            var attr = this.GetType().GetCustomAttribute<DataScriptAttribute>();
            if (attr != null)
            {
                this.key = attr.Key;
                if (attr.Key == null)
                    this.key = DataScriptAttribute.AutoKeyFor(this.GetType());
                    
                this.Expiration = TimeSpan.FromSeconds(attr.CacheDuration);
                this.Permission = attr.Permission;
                this.GroupKey = attr.CacheGroupKey;
            }
        }

        protected abstract TData GetData();
    }
}