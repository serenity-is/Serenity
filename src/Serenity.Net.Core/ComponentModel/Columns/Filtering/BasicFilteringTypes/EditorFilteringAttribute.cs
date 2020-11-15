using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that property should a custom editor for filtering,
    /// which is usually determined by form editor type of the property.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomFilteringAttribute" />
    public class EditorFilteringAttribute : CustomFilteringAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorFilteringAttribute"/> class.
        /// </summary>
        public EditorFilteringAttribute()
            : base("Editor")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorFilteringAttribute"/> class.
        /// </summary>
        /// <param name="editorType">Type of the editor.</param>
        public EditorFilteringAttribute(string editorType)
            : base("Editor")
        {
            EditorType = editorType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorFilteringAttribute"/> class.
        /// </summary>
        /// <param name="editorTypeAttribute">The editor type attribute to read editor type from.</param>
        public EditorFilteringAttribute(Type editorTypeAttribute)
            : base("Editor")
        {
            EditorType = ((EditorTypeAttribute)Activator.CreateInstance(editorTypeAttribute)).EditorType;
        }

        /// <summary>
        /// Gets or sets the type of the editor.
        /// </summary>
        /// <value>
        /// The type of the editor.
        /// </value>
        public String EditorType
        {
            get { return GetOption<String>("editorType"); }
            set { SetOption("editorType", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use relative comparisons, like GT/LT.
        /// </summary>
        /// <value>
        ///   <c>true</c> if should use relative comparisons; otherwise, <c>false</c>.
        /// </value>
        public Boolean UseRelative
        {
            get { return GetOption<Boolean>("useRelative"); }
            set { SetOption("useSearch", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use LIKE kind of operators including
        /// starts with, ends with etc.
        /// </summary>
        /// <value>
        ///   <c>true</c> if should use LIKE operators; otherwise, <c>false</c>.
        /// </value>
        public Boolean UseLike
        {
            get { return GetOption<Boolean>("useLike"); }
            set { SetOption("useLike", value); }
        }
    }
}