
namespace Serenity.ComponentModel
{
    /// <summary>
    /// Indicates that property should use integer type of filtering
    /// </summary>
    /// <seealso cref="Serenity.ComponentModel.CustomFilteringAttribute" />
    public class IntegerFilteringAttribute : CustomFilteringAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerFilteringAttribute"/> class.
        /// </summary>
        public IntegerFilteringAttribute()
            : base("Integer")
        {
        }
    }
}