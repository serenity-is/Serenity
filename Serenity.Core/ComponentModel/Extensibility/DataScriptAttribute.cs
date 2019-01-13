using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that this method / type should generate a remote data
    /// script. Data contained by remote data scripts can be access
    /// client side using Q.getRemoteData("Key") function.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.DynamicScriptAttribute" />
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple=false)]
    public class DataScriptAttribute : DynamicScriptAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataScriptAttribute"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public DataScriptAttribute(string key)
            : base("RemoteData." + key)
        {
        }
    }
}