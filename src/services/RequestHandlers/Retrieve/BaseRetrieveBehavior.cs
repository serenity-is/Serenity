namespace Serenity.Services;

/// <summary>
/// Base class for behaviors implementing <see cref="IRetrieveBehavior"/>
/// </summary>
public abstract class BaseRetrieveBehavior : IRetrieveBehavior
{
    /// <inheritdoc/>
    public virtual void OnValidateRequest(IRetrieveRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnPrepareQuery(IRetrieveRequestHandler handler, SqlQuery query)
    {
    }

    /// <inheritdoc/>
    public virtual void OnApplyFilters(IRetrieveRequestHandler handler, SqlQuery query)
    {
    }

    /// <inheritdoc/>
    public virtual void OnBeforeExecuteQuery(IRetrieveRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnAfterExecuteQuery(IRetrieveRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnReturn(IRetrieveRequestHandler handler)
    {
    }
}