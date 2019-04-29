using System;
using System.Globalization;

namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that the target property should use a "Decimal" editor.
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomEditorAttribute" />
    public partial class DecimalEditorAttribute : CustomEditorAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalEditorAttribute"/> class.
        /// </summary>
        public DecimalEditorAttribute()
            : base("Decimal")
        {
            if (AllowNegativesByDefault)
                AllowNegatives = true;
        }

        /// <summary>
        /// Gets or sets the number of decimals allowed.
        /// </summary>
        /// <value>
        /// The decimals allowed.
        /// </value>
        public Int32 Decimals
        {
            get { return GetOption<Int32>("decimals"); }
            set { SetOption("decimals", value); }
        }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        public Object MaxValue
        {
            get { return GetOption<String>("maxValue"); }
            set { SetOption("maxValue", value == null ? null : Convert.ToString(value, CultureInfo.InvariantCulture)); }
        }

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        public Object MinValue
        {
            get { return GetOption<String>("minValue"); }
            set { SetOption("minValue", value == null ? null : Convert.ToString(value, CultureInfo.InvariantCulture)); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to pad decimals with zero.
        /// </summary>
        /// <value>
        ///   <c>true</c> if pad decimals with zero; otherwise, <c>false</c>.
        /// </value>
        public Boolean PadDecimals
        {
            get { return GetOption<Boolean>("padDecimals"); }
            set { SetOption("padDecimals", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow negatives.
        /// </summary>
        /// <value>
        ///   <c>true</c> if should allow negatives; otherwise, <c>false</c>.
        /// </value>
        public Boolean AllowNegatives
        {
            get { return GetOption<Boolean>("allowNegatives"); }
            set { SetOption("allowNegatives", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow negatives by default.
        /// This is a global setting that controls if decimal editors should allow
        /// negative values unless specified otherwise.
        /// </summary>
        /// <value>
        ///   <c>true</c> if negatives should be allowed by default; otherwise, <c>false</c>.
        /// </value>
        public static bool AllowNegativesByDefault { get; set; }
    }
}