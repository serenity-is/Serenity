namespace Serenity.Services;

/// <summary>
/// Behavior that handles <see cref="UniqueAttribute"/> on a single field. It checks the
/// unique constraint before save (unless <see cref="UniqueAttribute.CheckBeforeSave"/> is
/// disabled) by running an existence query. Override <see cref="BaseUniqueConstraintSaveBehavior.BuildUniqueConstraintQuery"/>
/// in a derived behavior, or add your own behavior and set <see cref="UniqueAttribute.DisableDefaultBehavior"/>
/// on the field, to customize or replace the pre-check.
/// </summary>
/// <remarks>
/// Initializes a new instance of the class.
/// </remarks>
/// <param name="localizer">Text localizer</param>
public class UniqueFieldSaveBehavior(ITextLocalizer localizer) : BaseUniqueConstraintSaveBehavior, IImplicitBehavior, IFieldBehavior
{
    /// <inheritdoc/>
    public Field? Target { get; set; }

    private readonly ITextLocalizer localizer = localizer;

    private UniqueAttribute? attr;

    /// <inheritdoc/>
    public bool ActivateFor(IRow row)
    {
        if (Target is null)
            return false;

        if (!Target.Flags.HasFlag(FieldFlags.Unique))
            return false;

        var attr = Target.GetAttribute<UniqueAttribute>();
        if (attr != null && (!attr.CheckBeforeSave || attr.DisableDefaultBehavior))
            return false;

        this.attr = attr;
        return true;
    }

    /// <inheritdoc/>
    public virtual void OnBeforeSave(ISaveRequestHandler handler)
    {
        if (attr?.IgnoreNulls == true &&
            Target!.IsNull(handler.Row))
            return;

        ValidateUniqueConstraint(handler, [Target!], localizer,
            attr?.ErrorMessage,
            attr != null && attr.IgnoreDeleted ? ServiceQueryHelper.GetNotDeletedCriteria(handler.Row) : Criteria.Empty);
    }

    /// <inheritdoc/>
    public override async Task OnBeforeSaveAsync(ISaveRequestHandler handler, CancellationToken cancellationToken = default)
    {
        if (attr?.IgnoreNulls == true &&
            Target!.IsNull(handler.Row))
            return;

        await ValidateUniqueConstraintAsync(handler, [Target!], localizer,
            attr?.ErrorMessage,
            attr != null && attr.IgnoreDeleted ? ServiceQueryHelper.GetNotDeletedCriteria(handler.Row) : Criteria.Empty,
            cancellationToken).ConfigureAwait(false);
    }
}
