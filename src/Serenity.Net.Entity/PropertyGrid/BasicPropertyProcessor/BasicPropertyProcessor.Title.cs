namespace Serenity.PropertyGrid
{
    public partial class BasicPropertyProcessor : PropertyProcessor
    {
        private void SetTitle(IPropertySource source, PropertyItem item)
        {
            if (source.Property != null)
            {
                var attr = source.Property.GetCustomAttribute<DisplayNameAttribute>(false);
                if (attr != null)
                    item.Title = attr.DisplayName;
            }

            if (item.Title == null)
            {
                var basedOnField = source.BasedOnField;

                if (basedOnField is not null)
                {
                    item.Title = basedOnField.Caption is not null ?
                        basedOnField.Caption.Key : basedOnField.AutoTextKey;
                }
                else
                    item.Title = item.Name;
            }
        }
    }
}