namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetInsertable(IPropertySource source, PropertyItem item)
    {
        if (source.Property != null)
        {
            var attr = source.Property.GetAttribute<InsertableAttribute>(false);
            if (attr != null)
            {
                if (!attr.Value)
                    item.Insertable = false;

                return;
            }
        }

        if (source.BasedOnField is not null)
        {
            if ((source.BasedOnField.Flags & FieldFlags.Insertable) != FieldFlags.Insertable)
                item.Insertable = false;
        }
    }
}