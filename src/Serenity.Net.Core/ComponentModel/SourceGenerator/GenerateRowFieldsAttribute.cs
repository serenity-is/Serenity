namespace Serenity.ComponentModel
{
    /// <summary>
    /// Enables source generator for this row class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class GenerateRowFieldsAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateRowFieldsAttribute"/> class.
        /// </summary>
        public GenerateRowFieldsAttribute()
        {
        }
    }
}