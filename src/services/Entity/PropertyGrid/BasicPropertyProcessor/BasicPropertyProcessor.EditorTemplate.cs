namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    /// <summary>
    /// Sets the editor parameters from an editor attribute, after the parameters of its template,
    /// so that the options set on the attribute override the ones of the template.
    /// </summary>
    /// <param name="editorTypeAttr">The editor attribute.</param>
    /// <param name="editorParams">The editor parameters.</param>
    private static void SetEditorParams(EditorTypeAttribute editorTypeAttr, IDictionary<string, object?> editorParams)
    {
        GetEditorTemplate(editorTypeAttr)?.SetParams(editorParams);
        editorTypeAttr.SetParams(editorParams);
    }

    /// <summary>
    /// Gets the template of an editor attribute, which is an editor attribute of the same family applied to
    /// the class of the editor attribute, e.g. a <see cref="ServiceLookupEditorAttribute"/> on the partial
    /// class of an attribute generated for a custom service lookup editor. The editor type of the template is
    /// not used, only its options, and for service lookup editors its item type, endpoint type and action name.
    /// </summary>
    /// <param name="editorTypeAttr">The editor attribute.</param>
    /// <returns>The template, or null if there is none, or it is not of the same editor family.</returns>
    internal static EditorTypeAttribute? GetEditorTemplate(EditorTypeAttribute editorTypeAttr)
    {
        ArgumentNullException.ThrowIfNull(editorTypeAttr);

        var template = editorTypeAttr.GetType().GetCustomAttribute<EditorTypeAttribute>(inherit: true);
        if (template is null ||
            !IsSameEditorFamily(template.GetType(), editorTypeAttr.GetType()))
            return null;

        return template;
    }

    /// <summary>
    /// Checks whether two editor attribute types share a base class more specific than
    /// <see cref="CustomEditorAttribute"/>, like <see cref="ServiceLookupEditorBaseAttribute"/>,
    /// so that the options of one are meaningful for the other.
    /// </summary>
    private static bool IsSameEditorFamily(Type templateType, Type editorType)
    {
        for (var type = templateType;
            type is not null && type != typeof(CustomEditorAttribute) && type != typeof(EditorTypeAttribute);
            type = type.BaseType)
        {
            if (type.IsAssignableFrom(editorType))
                return true;
        }

        return false;
    }
}
