
namespace Serenity.Data.Mapping
{
    public class IdentityAttribute : SetFieldFlagsAttribute
    {
        public IdentityAttribute()
            : base(FieldFlags.Identity, FieldFlags.Insertable | FieldFlags.Updatable)
        {
        }
    }
}