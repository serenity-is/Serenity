using System;
using System.Reflection;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that the target property should use a "AsyncLookup" editor.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomEditorAttribute" />
    public partial class AsyncLookupEditorAttribute : CustomEditorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLookupEditorAttribute"/> class.
        /// </summary>
        /// <param name="lookupKey">The lookup key.</param>
        public AsyncLookupEditorAttribute(string lookupKey)
            : base("AsyncLookup")
        {
            SetOption("lookupKey", lookupKey);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncLookupEditorAttribute"/> class.
        /// </summary>
        /// <param name="lookupType">Type of the lookup.</param>
        /// <exception cref="ArgumentNullException">lookupType</exception>
        /// <exception cref="ArgumentException">lookupType</exception>
        public AsyncLookupEditorAttribute(Type lookupType)
            : base("AsyncLookup")
        {
            if (lookupType == null)
                throw new ArgumentNullException("lookupType");

            var attr = lookupType.GetCustomAttribute<LookupScriptAttribute>(false);
            if (attr == null)
            {
                throw new ArgumentException(String.Format(
                    "'{0}' type doesn't have a [LookupScript] attribute, so it can't " +
                    "be used with a AsyncLookupEditor!",
                    lookupType.FullName), "lookupType");
            }

            SetOption("lookupKey", attr.Key ??
                LookupScriptAttribute.AutoLookupKeyFor(lookupType));
        }

        /// <summary>
        /// Gets the lookup key.
        /// </summary>
        /// <value>
        /// The lookup key.
        /// </value>
        public string LookupKey
        {
            get { return GetOption<string>("lookupKey"); }
        }
    }
}