using Serenity.ComponentModel;
using Serenity.Localization;

namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    /// <summary>
    /// Returns the local text key that the form or column of a property implies for an item of its select editor, which
    /// <see cref="PropertyItemsLocalTextRegistration"/> registers with the text of the item as its default text, e.g.
    /// <c>Forms.Namespace.MyForm.Priority_SelectItem_low</c>.
    /// </summary>
    /// <param name="source">The property source.</param>
    /// <param name="item">The property item.</param>
    /// <param name="attribute">The item attribute.</param>
    /// <returns>The implied key, or null when the type of the property has no form or column prefix.</returns>
    private static string? ImpliedTextKey(IPropertySource source, PropertyItem item, SelectItemAttribute attribute)
    {
        if (source.ReflectedType is not Type reflectedType)
            return null;

        var prefix = PropertyItemsLocalTextRegistration.GetPropertyItemsTextPrefix(reflectedType);
        if (prefix == null)
            return null;

        return prefix + (source.Property?.Name ?? item.Name) + "_SelectItem_" + attribute.Value;
    }

    /// <summary>
    /// Sets the items of a "Select" editor from the <see cref="SelectItemAttribute"/> attributes of the property, if it
    /// has any, which imply the editor, along with the field of their text keys when they have any.
    /// </summary>
    /// <param name="source">The property source.</param>
    /// <param name="item">The property item.</param>
    private static void SetSelectItems(IPropertySource source, PropertyItem item)
    {
        var attributes = source.GetAttributes<SelectItemAttribute>().ToList();
        if (attributes.Count == 0)
            return;

        // PropertyItemsLocalTextRegistration registers the implied keys of the items of the member of the form or
        // columns class, and not of the row field it is based on, whose items would get a key that has no text; the
        // field keeps its attributes, so they are the same instances as the ones above
        var rowAttributes = new HashSet<object>(
            source.BasedOnField?.CustomAttributes?.OfType<SelectItemAttribute>() ?? [],
            ReferenceEqualityComparer.Instance);

        var editorTypeAttr = source.GetAttribute<EditorTypeAttribute>();
        if (editorTypeAttr != null && editorTypeAttr.EditorType != SelectEditorAttribute.Key)
        {
            throw new InvalidOperationException(string.Format(
                "The property '{0}' of '{1}' has items of a select editor, but its editor is '{2}'. Use the items on " +
                "a property whose editor is a select, or leave the editor out, as the items imply it.",
                source.Property?.Name ?? item.Name,
                source.ReflectedType?.FullName ?? source.Property?.DeclaringType?.FullName,
                editorTypeAttr.EditorType));
        }

        if (editorTypeAttr == null)
        {
            // the options of the editor that was determined from the type of the property, e.g. of a number, do not
            // apply to the editor that the items give
            item.EditorParams.Remove("allowNegatives");
            item.EditorParams.Remove("maxValue");
            item.EditorType = SelectEditorAttribute.Key;
        }

        var items = new List<Dictionary<string, object?>>();
        var ids = new HashSet<string>(StringComparer.Ordinal);

        // the attributes of a property come in no order that is promised, which is what the order of an item is for,
        // and the ones with the same order keep the order they came in
        var ordered = attributes.Select((attribute, index) => (attribute, index))
            .OrderBy(x => x.attribute.Order).ThenBy(x => x.index).Select(x => x.attribute);

        foreach (var attribute in ordered)
        {
            if (!ids.Add(attribute.Value))
            {
                throw new InvalidOperationException(string.Format(
                    "The property '{0}' of '{1}' has more than one item of the select editor with the value '{2}'.",
                    source.Property?.Name ?? item.Name,
                    source.ReflectedType?.FullName ?? source.Property?.DeclaringType?.FullName,
                    attribute.Value));
            }

            var selectItem = new Dictionary<string, object?>
            {
                ["id"] = attribute.Value
            };

            // an item without a text key shows its value, which is the text of the item; one with a text key falls back
            // to what the editor decides, as the text of a key is not the key itself
            var text = !string.IsNullOrEmpty(attribute.Text) ? attribute.Text :
                string.IsNullOrEmpty(attribute.TextKey) ? attribute.Value : null;
            if (text != null)
                selectItem["text"] = text;

            // an item that has a text, but no key of its own, takes the key that its form implies for it, e.g.
            // Priority_SelectItem_low, so that its text can be localized without a change to the code of the form,
            // while an empty key leaves the item as it is, e.g. one whose text is a name, see ResolveFromAttribute, as
            // does an item of the row field that the property is based on, which has no form to imply a key
            var resolved = LocalText.ResolveFromAttribute(attribute, attribute.Text,
                string.IsNullOrEmpty(attribute.Text) || rowAttributes.Contains(attribute) ? null :
                    ImpliedTextKey(source, item, attribute));
            var textKey = resolved.Literal ? null : resolved.Key;
            if (textKey != null)
                selectItem["textKey"] = textKey;

            if (!string.IsNullOrEmpty(attribute.Icon))
                selectItem["icon"] = attribute.Icon;

            if (!string.IsNullOrEmpty(attribute.CssClass))
                selectItem["cssClass"] = attribute.CssClass;

            items.Add(selectItem);
        }

        item.EditorParams["items"] = items;

        // the items are told apart from their texts, which are keys, only when they have any
        if (!item.EditorParams.ContainsKey("textKeyField") && items.Any(x => x.ContainsKey("textKey")))
            item.EditorParams["textKeyField"] = "textKey";
    }
}
