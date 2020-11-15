namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that the target property should use a "String" editor.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomEditorAttribute" />
    public partial class StringEditorAttribute : CustomEditorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringEditorAttribute"/> class.
        /// </summary>
        public StringEditorAttribute()
            : base("String")
        {
        }
    }
}