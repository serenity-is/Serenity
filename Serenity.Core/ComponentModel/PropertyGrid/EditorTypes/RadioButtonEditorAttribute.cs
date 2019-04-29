using System;
using System.Reflection;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that the target property should use a "RadioButton" editor.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomEditorAttribute" />
    public partial class RadioButtonEditorAttribute : CustomEditorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButtonEditorAttribute"/> class.
        /// </summary>
        /// <param name="enumOrLookupType">Type of the enum or lookup.</param>
        /// <exception cref="ArgumentNullException">enumOrLookupType</exception>
        /// <exception cref="ArgumentException">lookupType</exception>
        public RadioButtonEditorAttribute(Type enumOrLookupType)
            : base("RadioButton")
        {
            if (enumOrLookupType == null)
                throw new ArgumentNullException("enumOrLookupType");

            if (enumOrLookupType.IsEnum)
            {
                var ek = enumOrLookupType.GetCustomAttribute<EnumKeyAttribute>(false);
                if (ek == null)
                    EnumKey = enumOrLookupType.FullName;
                else
                    EnumKey = ek.Value;

                return;
            }

            var lk = enumOrLookupType.GetCustomAttribute<LookupScriptAttribute>(false);
            if (lk == null)
            {
                throw new ArgumentException(String.Format(
                    "'{0}' type doesn't have a [LookupScript] attribute, so it can't " +
                    "be used with a RadioButtonEditor!",
                    enumOrLookupType.FullName), "lookupType");
            }

            LookupKey = lk.Key ?? LookupScriptAttribute.AutoLookupKeyFor(enumOrLookupType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButtonEditorAttribute"/> class.
        /// </summary>
        public RadioButtonEditorAttribute()
            : base("RadioButton")
        {
        }

        /// <summary>
        /// Gets or sets the enum key.
        /// </summary>
        /// <value>
        /// The enum key.
        /// </value>
        public String EnumKey
        {
            get { return GetOption<String>("enumKey"); }
            set { SetOption("enumKey", value); }
        }

        /// <summary>
        /// Gets or sets the lookup key.
        /// </summary>
        /// <value>
        /// The lookup key.
        /// </value>
        public String LookupKey
        {
            get { return GetOption<String>("lookupKey"); }
            set { SetOption("lookupKey", value); }
        }
    }
}