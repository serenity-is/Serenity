namespace Serenity.Services;

/// <summary>
/// Base class for types implementing <see cref="IUndeleteBehavior"/>
/// </summary>
public abstract class BaseUndeleteBehavior : IUndeleteBehavior, IUndeleteExceptionBehavior
{
    /// <inheritdoc/>
    public virtual void OnPrepareQuery(IUndeleteRequestHandler handler, SqlQuery query)
    {
    }

    /// <inheritdoc/>
    public virtual void OnValidateRequest(IUndeleteRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnBeforeUndelete(IUndeleteRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnAfterUndelete(IUndeleteRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnAudit(IUndeleteRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnReturn(IUndeleteRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnException(IUndeleteRequestHandler handler, Exception exception)
    {
    }
}