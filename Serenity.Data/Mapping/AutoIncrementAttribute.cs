
namespace Serenity.Data.Mapping
{
    public class AutoIncrementAttribute : SetFieldFlagsAttribute
    {
        public AutoIncrementAttribute()
            : base(FieldFlags.AutoIncrement, FieldFlags.Insertable | FieldFlags.Updatable)
        {
        }
    }
}