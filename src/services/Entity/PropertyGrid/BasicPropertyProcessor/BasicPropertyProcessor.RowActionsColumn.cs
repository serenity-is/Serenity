namespace Serenity.PropertyGrid;

public partial class BasicPropertyProcessor : PropertyProcessor
{
    private static void SetRowActionsColumn(IPropertySource source, PropertyItem item)
    {
        if (source.GetAttribute<RowActionsColumnAttribute>() == null)
            return;

        // the column of the buttons of the row actions is found by its name, like the one of the row selection
        if (source.Property?.Name != RowActionsColumnAttribute.ColumnName)
        {
            throw new InvalidProgramException(string.Format(
                "The property '{0}' of '{1}' has a [RowActionsColumn] attribute, but must be named '{2}', " +
                "as the client finds the column of the row actions by its name.",
                source.Property?.Name, source.Property?.DeclaringType?.FullName,
                RowActionsColumnAttribute.ColumnName));
        }
    }
}
