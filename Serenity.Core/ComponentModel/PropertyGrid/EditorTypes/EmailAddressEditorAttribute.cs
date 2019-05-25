namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that the target property should use a "EmailAddress" editor.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomEditorAttribute" />
    public partial class EmailAddressEditorAttribute : CustomEditorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddressEditorAttribute"/> class.
        /// </summary>
        public EmailAddressEditorAttribute()
            : base("EmailAddress")
        {
        }
    }
}