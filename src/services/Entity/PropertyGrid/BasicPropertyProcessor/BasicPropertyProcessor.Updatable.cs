namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetUpdatable(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<UpdatableAttribute>(AttributeOrigin.ExcludeBasedOnField);
        if (attr != null)
        {
            if (!attr.Value)
                item.Updatable = false;

            return;
        }

        if (source.BasedOnField is not null)
        {
            if ((source.BasedOnField.Flags & FieldFlags.Updatable) != FieldFlags.Updatable)
                item.Updatable = false;
        }
    }
}