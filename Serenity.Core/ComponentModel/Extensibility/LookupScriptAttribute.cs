using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Placed on rows / or custom lookup classes to denote
    /// it has a lookup script.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public sealed class LookupScriptAttribute : Attribute
    {
        /// <summary>
        /// Creates a LookupScriptAttribute.
        /// </summary>
        /// <param name="key">Lookup key, usually in "Module.EntityName" format.</param>
        public LookupScriptAttribute(string key)
        {
            this.Key = key;
        }

        /// <summary>
        /// Lookup key, usually in "Module.EntityName" format.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Permission key required to access this lookup script.
        /// Use special value "?" for all logged-in users.
        /// Use special value "*" for anyone including not logged-in users.
        /// </summary>
        public string Permission { get; set; }

        /// <summary>
        /// Cache duration in seconds
        /// </summary>
        public int Expiration { get; set; }
    }
}