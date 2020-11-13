#if !NET
using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that the target property should use a "Recaptcha" (Google).
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomEditorAttribute" />
#if !NET45
    [Obsolete("Dependends on static configuration")]
#endif
    public partial class RecaptchaAttribute : CustomEditorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecaptchaAttribute"/> class.
        /// </summary>
        public RecaptchaAttribute()
            : base("Recaptcha")
        {
            var settings = Config.TryGet<RecaptchaSettings>();
            if (settings != null)
                SiteKey = settings.SiteKey;
        }

        /// <summary>
        /// Gets or sets the site key.
        /// </summary>
        /// <value>
        /// The site key.
        /// </value>
        public String SiteKey
        {
            get { return GetOption<String>("siteKey"); }
            set { SetOption("siteKey", value); }
        }
    }
}
#endif