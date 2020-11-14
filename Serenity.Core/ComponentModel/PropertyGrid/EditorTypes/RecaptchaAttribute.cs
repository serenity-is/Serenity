namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that the target property should use a "Recaptcha" (Google).
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomEditorAttribute" />
    public partial class RecaptchaAttribute : CustomEditorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecaptchaAttribute"/> class.
        /// </summary>
        public RecaptchaAttribute()
            : base("Recaptcha")
        {
        }
    }
}