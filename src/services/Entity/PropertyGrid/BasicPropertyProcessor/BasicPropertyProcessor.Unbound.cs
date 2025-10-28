namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetUnbound(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<UnboundAttribute>();
        if (attr != null)
            item.Unbound = true;
    }
}