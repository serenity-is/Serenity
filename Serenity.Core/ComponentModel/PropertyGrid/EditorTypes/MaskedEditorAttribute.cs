using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that the target property should use a "Masked" editor
    /// and also defines an automatic lookup script for row fields.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomEditorAttribute" />
    public partial class MaskedEditorAttribute : CustomEditorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaskedEditorAttribute"/> class.
        /// </summary>
        public MaskedEditorAttribute()
            : base("Masked")
        {
        }

        /// <summary>
        /// Gets or sets the mask. a = letter, 9 = numeric, * = alphanumeric.
        /// </summary>
        /// <value>
        /// The mask.
        /// </value>
        public String Mask
        {
            get { return GetOption<String>("mask"); }
            set { SetOption("mask", value); }
        }

        /// <summary>
        /// Gets or sets the placeholder.
        /// </summary>
        /// <value>
        /// The placeholder.
        /// </value>
        public String Placeholder
        {
            get { return GetOption<String>("placeholder"); }
            set { SetOption("placeholder", value); }
        }
    }
}