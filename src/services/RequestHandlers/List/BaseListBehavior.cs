namespace Serenity.Services;

/// <summary>
/// Base class for behaviors implementing <see cref="IListBehavior"/>
/// </summary>
public abstract class BaseListBehavior : IListBehavior
{
    /// <inheritdoc/>
    public virtual void OnValidateRequest(IListRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnPrepareQuery(IListRequestHandler handler, SqlQuery query)
    {
    }

    /// <inheritdoc/>
    public virtual void OnApplyFilters(IListRequestHandler handler, SqlQuery query)
    {
    }

    /// <inheritdoc/>
    public virtual void OnBeforeExecuteQuery(IListRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnAfterExecuteQuery(IListRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnReturn(IListRequestHandler handler)
    {
    }
}