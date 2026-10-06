namespace Serenity.Services;

/// <summary>
/// Behavior that handles <see cref="UniqueConstraintAttribute"/> on a row. It checks each
/// unique constraint before save (unless <see cref="UniqueConstraintAttribute.CheckBeforeSave"/>
/// is disabled) by running an existence query. Override <see cref="BaseUniqueConstraintSaveBehavior.BuildUniqueConstraintQuery"/>
/// in a derived behavior, or add your own behavior and set <see cref="UniqueConstraintAttribute.DisableDefaultBehavior"/>,
/// to customize or replace the pre-check.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="localizer">Text localizer</param>
public class UniqueConstraintSaveBehavior(ITextLocalizer localizer) : BaseUniqueConstraintSaveBehavior, IImplicitBehavior
{
    private UniqueConstraintAttribute[]? attrList;
    private IEnumerable<Field>[]? attrFields;
    private readonly ITextLocalizer localizer = localizer;

    /// <inheritdoc/>
    public bool ActivateFor(IRow row)
    {
        var attr = row.GetType().GetCustomAttributes<UniqueConstraintAttribute>()
            .Where(x => x.CheckBeforeSave && !x.DisableDefaultBehavior);

        if (!attr.Any())
            return false;

        attrList = [.. attr];
        return true;
    }

    /// <inheritdoc/>
    public virtual void OnBeforeSave(ISaveRequestHandler handler)
    {
        if (attrList == null)
            return;

        EnsureAttrFields(handler);

        for (var i = 0; i < attrList.Length; i++)
        {
            var attr = attrList[i];
            var fields = attrFields![i];

            ValidateUniqueConstraint(handler, fields, localizer, attr.ErrorMessage,
                attrList[i].IgnoreDeleted ? ServiceQueryHelper.GetNotDeletedCriteria(handler.Row) : Criteria.Empty);
        }
    }

    /// <inheritdoc/>
    public override async Task OnBeforeSaveAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        if (attrList == null)
            return;

        EnsureAttrFields(handler);

        for (var i = 0; i < attrList.Length; i++)
        {
            var attr = attrList![i];
            var fields = attrFields![i];

            await ValidateUniqueConstraintAsync(handler, fields, localizer, attr.ErrorMessage,
                attrList[i].IgnoreDeleted ? ServiceQueryHelper.GetNotDeletedCriteria(handler.Row) : Criteria.Empty,
                cancellationToken).ConfigureAwait(false);
        }
    }

    private void EnsureAttrFields(ISaveRequestHandler handler)
    {
        if (attrFields != null)
            return;

        attrFields = [.. attrList!.Select(attr =>
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
        })];
    }
}
