namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetUpdatable(IPropertySource source, PropertyItem item)
    {
        if (source.Property != null)
        {
            var attr = source.Property.GetAttribute<UpdatableAttribute>(false);
            if (attr != null)
            {
                if (!attr.Value)
                    item.Updatable = false;

                return;
            }
        }

        if (source.BasedOnField is not null)
        {
            if ((source.BasedOnField.Flags & FieldFlags.Updatable) != FieldFlags.Updatable)
                item.Updatable = false;
        }
    }
}