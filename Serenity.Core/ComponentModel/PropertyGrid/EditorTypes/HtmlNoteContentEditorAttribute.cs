using System;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that the target property should use a "HtmlNoteContent" editor.
    /// This is generally a CK editor with only basic functionality for notes enabled.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomEditorAttribute" />
    public partial class HtmlNoteContentEditorAttribute : CustomEditorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlNoteContentEditorAttribute"/> class.
        /// </summary>
        public HtmlNoteContentEditorAttribute()
            : base("HtmlNoteContent")
        {
        }

        /// <summary>
        /// Gets or sets the cols of underlying textarea.
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
        /// Gets or sets the rows of underlying textarea.
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