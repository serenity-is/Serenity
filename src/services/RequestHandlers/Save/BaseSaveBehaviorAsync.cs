namespace Serenity.Services;

/// <summary>
/// Base class for behaviors implementing <see cref="ISaveBehaviorAsync"/>
/// </summary>
public abstract class BaseSaveBehaviorAsync : ISaveBehaviorAsync
{
    /// <inheritdoc/>
    public virtual Task OnPrepareQueryAsync(ISaveRequestHandler handler, SqlQuery query,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnValidateRequestAsync(ISaveRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnSetInternalFieldsAsync(ISaveRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnBeforeSaveAsync(ISaveRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnAfterSaveAsync(ISaveRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnAuditAsync(ISaveRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnReturnAsync(ISaveRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
