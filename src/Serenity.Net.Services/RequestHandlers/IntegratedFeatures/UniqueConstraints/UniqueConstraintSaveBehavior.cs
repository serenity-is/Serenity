namespace Serenity.Services;

/// <summary>
/// Behavior that handles <see cref="UniqueConstraintAttribute"/>
/// </summary>
public class UniqueConstraintSaveBehavior : BaseSaveBehavior, IImplicitBehavior
{
    private UniqueConstraintAttribute[] attrList;
    private IEnumerable<Field>[] attrFields;
    private readonly ITextLocalizer localizer;

    /// <summary>
    /// Creates a new instance of the class
    /// </summary>
    /// <param name="localizer">Text localizer</param>
    public UniqueConstraintSaveBehavior(ITextLocalizer localizer)
    {
        this.localizer = localizer;
    }

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

        if (attrFields == null)
        {
            attrFields = attrList.Select(attr =>
            {
                return attr.Fields.Select(x =>
                {
                    var field = handler.Row.FindFieldByPropertyName(x) ?? handler.Row.FindField(x);
                    if (ReferenceEquals(null, field))
                    {
                        throw new InvalidOperationException(string.Format(
                            "Can't find field '{0}' of unique constraint in row type '{1}'",
                                x, handler.Row.GetType().FullName));
                    }
                    return field;
                });
            }).ToArray();
        }

        for (var i = 0; i < attrList.Length; i++)
        {
            var attr = attrList[i];
            var fields = attrFields[i];

            UniqueFieldSaveBehavior.ValidateUniqueConstraint(handler, fields, localizer, attr.ErrorMessage,
                attrList[i].IgnoreDeleted ? ServiceQueryHelper.GetNotDeletedCriteria(handler.Row) : Criteria.Empty);
        }
    }
}