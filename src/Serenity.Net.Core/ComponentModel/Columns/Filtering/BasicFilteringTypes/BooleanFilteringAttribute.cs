using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Sets filtering type to "Boolean"
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomFilteringAttribute" />
    public class BooleanFilteringAttribute : CustomFilteringAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanFilteringAttribute"/> class.
        /// </summary>
        public BooleanFilteringAttribute()
            : base("Boolean")
        {
        }

        /// <summary>
        /// Gets or sets the text used of false value.
        /// </summary>
        /// <value>
        /// The false text.
        /// </value>
        public String FalseText
        {
            get { return GetOption<String>("falseText"); }
            set { SetOption("falseText", value); }
        }

        /// <summary>
        /// Gets or sets the text used for true value.
        /// </summary>
        /// <value>
        /// The true text.
        /// </value>
        public String TrueText
        {
            get { return GetOption<String>("trueText"); }
            set { SetOption("trueText", value); }
        }
    }
}