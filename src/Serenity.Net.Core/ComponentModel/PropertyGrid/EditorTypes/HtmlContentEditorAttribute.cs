using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that the target property should use a "HtmlContent" editor.
    /// This is generally a CK editor that contains more functionalities
    /// compared to other ones.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomEditorAttribute" />
    public partial class HtmlContentEditorAttribute : CustomEditorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlContentEditorAttribute"/> class.
        /// </summary>
        public HtmlContentEditorAttribute()
            : base("HtmlContent")
        {
        }

        /// <summary>
        /// Gets or sets the cols for underlying textarea.
        /// </summary>
        /// <value>
        /// The cols.
        /// </value>
        public Int32 Cols
        {
            get { return GetOption<Int32>("cols"); }
            set { SetOption("cols", value); }
        }


        /// <summary>
        /// Gets or sets the rows for underlying textarea.
        /// </summary>
        /// <value>
        /// The rows.
        /// </value>
        public Int32 Rows
        {
            get { return GetOption<Int32>("rows"); }
            set { SetOption("rows", value); }
        }
    }

}