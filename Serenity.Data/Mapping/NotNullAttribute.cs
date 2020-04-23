
namespace Serenity.Data.Mapping
{
    /// <summary>
    /// Specifies that field can not be null.
    /// </summary>
    /// <seealso cref="Serenity.Data.Mapping.SetFieldFlagsAttribute" />
    public class NotNullAttribute : SetFieldFlagsAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullAttribute"/> class.
        /// </summary>
        public NotNullAttribute()
            : base(FieldFlags.NotNull)
        {
        }
    }
}