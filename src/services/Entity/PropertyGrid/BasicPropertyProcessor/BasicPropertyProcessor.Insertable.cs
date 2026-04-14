namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetInsertable(IPropertySource source, PropertyItem item)
    {
        var attr = source.GetAttribute<InsertableAttribute>(AttributeOrigin.ExcludeBasedOnField);
        if (attr != null)
        {
            if (!attr.Value)
                item.Insertable = false;

            return;
        }

        if (source.BasedOnField is not null)
        {
            if ((source.BasedOnField.Flags & FieldFlags.Insertable) != FieldFlags.Insertable)
                item.Insertable = false;
        }
    }
}