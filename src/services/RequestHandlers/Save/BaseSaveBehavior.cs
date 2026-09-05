namespace Serenity.Services;

/// <summary>
/// Base class for behaviors implementing <see cref="ISaveBehaviorSync"/>
/// </summary>
public abstract class BaseSaveBehavior : ISaveBehaviorSync, ISaveExceptionBehavior
{
    /// <inheritdoc/>
    public virtual void OnPrepareQuery(ISaveRequestHandler handler, SqlQuery query)
    {
    }

    /// <inheritdoc/>
    public virtual void OnAfterSave(ISaveRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnBeforeSave(ISaveRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnAudit(ISaveRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnReturn(ISaveRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnSetInternalFields(ISaveRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnValidateRequest(ISaveRequestHandler handler)
    {
    }

    /// <inheritdoc/>
    public virtual void OnException(ISaveRequestHandler handler, Exception exception)
    {
    }
}