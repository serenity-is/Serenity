namespace Serenity.Extensions;

public class DefaultDBAttribute : TargetDBAttribute
{
    public DefaultDBAttribute()
        : base("Default")
    {
    }
}