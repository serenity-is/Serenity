using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that the target property should use a "DateYear" editor.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomEditorAttribute" />
    public partial class DateYearEditorAttribute : CustomEditorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateYearEditorAttribute"/> class.
        /// </summary>
        public DateYearEditorAttribute()
            : base("DateYear")
        {
        }

        /// <summary>
        /// Gets or sets the maximum year.
        /// It can be written as an integer value, or +50, -20 etc. 
        /// corresponding to current year plus or minus years.
        /// </summary>
        /// <value>
        /// The maximum year.
        /// </value>
        public String MaxYear
        {
            get { return GetOption<String>("maxYear"); }
            set { SetOption("maxYear", value); }
        }

        /// <summary>
        /// Gets or sets the minimum year.
        /// It can be written as an integer value, or +50, -20 etc. 
        /// corresponding to current year plus or minus years.
        /// </summary>
        /// <value>
        /// The minimum year. 
        /// </value>
        public String MinYear
        {
            get { return GetOption<String>("minYear"); }
            set { SetOption("minYear", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the years should be listed in descending order.
        /// </summary>
        /// <value>
        ///   <c>true</c> if descending; otherwise, <c>false</c>.
        /// </value>
        public Boolean Descending
        {
            get { return GetOption<Boolean>("descending"); }
            set { SetOption("descending", value); }
        }
    }
}