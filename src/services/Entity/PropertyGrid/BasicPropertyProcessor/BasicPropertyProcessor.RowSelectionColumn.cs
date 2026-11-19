namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetRowSelectionColumn(IPropertySource source, PropertyItem item)
    {
        if (source.GetAttribute<RowSelectionColumnAttribute>() == null)
            return;

        // the column of the check boxes that select the rows is found by its name, which is the id of the
        // column, so a property with the attribute must be named after it, like RowEditActionsColumn's
        // property, otherwise the client would show an ordinary column where a checkbox is expected
        if (source.Property?.Name != RowSelectionColumnAttribute.ColumnName)
        {
            throw new InvalidProgramException(string.Format(
                "The property '{0}' of '{1}' has a [RowSelectionColumn] attribute, but must be named '{2}', " +
                "as the client finds the column of the check boxes that select the rows by its name.",
                source.Property?.Name, source.Property?.DeclaringType?.FullName,
                RowSelectionColumnAttribute.ColumnName));
        }
    }
}
