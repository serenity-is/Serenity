namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private void SetEditorCssClass(IPropertySource source, PropertyItem item)
    {
        var cssClass = source.GetAttribute<EditorCssClassAttribute>();
        if (cssClass is not null &&
            !string.IsNullOrEmpty(cssClass.Value))
        {
            item.EditorCssClass = cssClass.Value;
        }
    }
}