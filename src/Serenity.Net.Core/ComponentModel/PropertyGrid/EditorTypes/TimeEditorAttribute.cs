using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that the target property should use a "Time" editor.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomEditorAttribute" />
    public partial class TimeEditorAttribute : CustomEditorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeEditorAttribute"/> class.
        /// </summary>
        public TimeEditorAttribute()
            : base("Time")
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether empty option should be shown.
        /// </summary>
        /// <value>
        ///   <c>true</c> if no empty option; otherwise, <c>false</c>.
        /// </value>
        public Boolean NoEmptyOption
        {
            get { return GetOption<Boolean>("noEmptyOption"); }
            set { SetOption("noEmptyOption", value); }
        }

        /// <summary>
        /// Gets or sets the start hour between 0 and 23.
        /// </summary>
        /// <value>
        /// The start hour.
        /// </value>
        public Int32 StartHour
        {
            get { return GetOption<Int32>("startHour"); }
            set { SetOption("startHour", value); }
        }

        /// <summary>
        /// Gets or sets the end hour between 0 and 23.
        /// </summary>
        /// <value>
        /// The end hour.
        /// </value>
        public Int32 EndHour
        {
            get { return GetOption<Int32>("endHour"); }
            set { SetOption("endHour", value); }
        }

        /// <summary>
        /// Gets or sets the interval minutes.
        /// </summary>
        /// <value>
        /// The interval minutes.
        /// </value>
        public Int32 IntervalMinutes
        {
            get { return GetOption<Int32>("intervalMinutes"); }
            set { SetOption("intervalMinutes", value); }
        }
    }
}