using System;
using System.Reflection;

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
        /// Defines that this type has an external lookup script type,
        /// and the lookup key is available on that type. Use this overload only
        /// for row types that have external lookups.
        /// </summary>
        /// <param name="lookupType">Script type with LookupScript attribute of its own.</param>
        public LookupScriptAttribute(Type lookupType)
        {
            if (lookupType == null)
                throw new ArgumentNullException("lookupType");

            var attr = lookupType.GetCustomAttribute<LookupScriptAttribute>();
            if (attr == null)
                throw new ArgumentOutOfRangeException("lookupType", String.Format(
                    "Type {0} is specified as lookup type in a LookupScript attribute, " + 
                    "but it has not LookupScript attribute itself.", lookupType.FullName));

            this.Key = attr.Key;
            this.LookupType = lookupType;
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

        /// <summary>
        /// External lookup script type or base type that should be used for generating dynamic lookup script.
        /// Only meaningfull for Row types with external lookup scripts. Can be a generic type of TRow or
        /// a simple lookup class.
        /// </summary>
        public Type LookupType { get; set; }
    }
}