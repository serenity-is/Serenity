
namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that this field should have decimal type of filtering
    /// </summary>
    public class DecimalFilteringAttribute : CustomFilteringAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalFilteringAttribute"/> class.
        /// </summary>
        public DecimalFilteringAttribute()
            : base("Decimal")
        {
        }
    }
}