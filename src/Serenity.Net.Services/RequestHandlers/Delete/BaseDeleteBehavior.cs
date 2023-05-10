namespace Serenity.Services;

/// <summary>
/// Base class for types implementing <see cref="IDeleteBehavior"/>
/// </summary>
public abstract class BaseDeleteBehavior : IDeleteBehavior, IDeleteExceptionBehavior
{
    /// <inheritdoc/>
    public virtual void OnPrepareQuery(IDeleteRequestHandler handler, SqlQuery query)
    {
    }

    /// <inheritdoc/>
    public virtual void OnValidateRequest(IDeleteRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnBeforeDelete(IDeleteRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnAfterDelete(IDeleteRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnAudit(IDeleteRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnReturn(IDeleteRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnException(IDeleteRequestHandler handler, Exception exception)
    {
    }
}