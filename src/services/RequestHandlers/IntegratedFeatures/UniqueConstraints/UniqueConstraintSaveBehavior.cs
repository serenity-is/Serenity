namespace Serenity.Services;

/// <summary>
/// Behavior that handles <see cref="UniqueConstraintAttribute"/>
/// </summary>
/// <remarks>
/// Creates a new instance of the class
/// </remarks>
/// <param name="localizer">Text localizer</param>
public class UniqueConstraintSaveBehavior(ITextLocalizer localizer) : BaseSaveBehavior, IImplicitBehavior
{
    private UniqueConstraintAttribute[] attrList;
    private IEnumerable<Field>[] attrFields;
    private readonly ITextLocalizer localizer = localizer;

    /// <inheritdoc/>
    public bool ActivateFor(IRow row)
    {
        var attr = row.GetType().GetCustomAttributes<UniqueConstraintAttribute>()
            .Where(x => x.CheckBeforeSave);

        if (!attr.Any())
            return false;

        attrList = attr.ToArray();
        return true;
    }

    /// <inheritdoc/>
    public override void OnBeforeSave(ISaveRequestHandler handler)
    {
        if (attrList == null)
            return;

        attrFields ??= attrList.Select(attr =>
            {
                return attr.Fields.Select(x =>
                {
                    var field = handler.Row.FindFieldByPropertyName(x) ?? handler.Row.FindField(x);
                    return field is null
                        ? throw new InvalidOperationException(string.Format(
                            "Can't find field '{0}' of unique constraint in row type '{1}'",
                                x, handler.Row.GetType().FullName))
                        : field;
                });
            }).ToArray();

        for (var i = 0; i < attrList.Length; i++)
        {
            var attr = attrList[i];
            var fields = attrFields[i];

            UniqueFieldSaveBehavior.ValidateUniqueConstraint(handler, fields, localizer, attr.ErrorMessage,
                attrList[i].IgnoreDeleted ? ServiceQueryHelper.GetNotDeletedCriteria(handler.Row) : Criteria.Empty);
        }
    }
}