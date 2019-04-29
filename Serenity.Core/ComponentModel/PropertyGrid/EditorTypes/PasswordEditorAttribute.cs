using System;
using System.Globalization;
using System.Reflection;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that the target property should use a "Password" editor.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomEditorAttribute" />
    public partial class PasswordEditorAttribute : CustomEditorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordEditorAttribute"/> class.
        /// </summary>
        public PasswordEditorAttribute()
            : base("Password")
        {
        }
    }
}