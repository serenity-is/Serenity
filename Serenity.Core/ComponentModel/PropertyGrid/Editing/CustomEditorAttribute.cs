using System.Collections.Generic;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Base attribute class that all other custom editor types
    /// derives from.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.EditorTypeAttribute" />
    public abstract class CustomEditorAttribute : EditorTypeAttribute
    {
        private Dictionary<string, object> options;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEditorAttribute"/> class.
        /// </summary>
        /// <param name="editorType">Type of the editor.</param>
        public CustomEditorAttribute(string editorType)
            : base(editorType)
        {
        }

        /// <summary>
        /// Sets the parameters for each pair in the editorParams dictionary.
        /// </summary>
        /// <param name="editorParams">The editor parameters.</param>
        public override void SetParams(IDictionary<string, object> editorParams)
        {
            if (options != null)
                foreach (var opt in options)
                    editorParams[opt.Key] = opt.Value;
        }

        /// <summary>
        /// Sets the editor option.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected void SetOption(string key, object value)
        {
            this.options = this.options ?? new Dictionary<string, object>();
            this.options[key] = value;
        }

        /// <summary>
        /// Gets the editor option.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected TType GetOption<TType>(string key)
        {
            if (this.options == null)
                return default(TType);

            object obj;
            if (!this.options.TryGetValue(key, out obj) || obj == null)
                return default(TType);

            return (TType)obj;
        }
    }
}