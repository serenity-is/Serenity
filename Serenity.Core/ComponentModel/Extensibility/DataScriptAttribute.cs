using System;

namespace Serenity.ComponentModel
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class DataScriptAttribute : DynamicScriptAttribute
    {
        public DataScriptAttribute(string key)
            : base("RemoteData." + key)
        {
        }
    }
}