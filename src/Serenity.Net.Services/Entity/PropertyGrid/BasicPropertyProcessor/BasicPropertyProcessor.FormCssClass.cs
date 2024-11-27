namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private FormCssClassAttribute formCssClassPrior;
    private FormWidthAttribute formWidthPrior;

    private void SetFormCssClass(IPropertySource source, PropertyItem item)
    {
        var cssClass = source.GetAttribute<FormCssClassAttribute>() ?? formCssClassPrior;

        if (cssClass != null)
        {
            if (!string.IsNullOrEmpty(cssClass.Value))
            {
                if (!string.IsNullOrEmpty(item.FormCssClass))
                    item.FormCssClass = " " + cssClass.Value;
                else
                    item.FormCssClass = cssClass.Value;
            }

            formCssClassPrior = cssClass.UntilNext ? cssClass : null;
        }

        var width = source.GetAttribute<FormWidthAttribute>() ?? formWidthPrior;
        if (width != null)
        {
            if (!string.IsNullOrEmpty(width.Value))
            {
                if (!string.IsNullOrEmpty(item.FormCssClass))
                    item.FormCssClass += " " + width.Value;
                else
                    item.FormCssClass = width.Value;
            }

            if (!width.JustThis)
                formWidthPrior = width.UntilNext ? width : null;
        }
    }
}