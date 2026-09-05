namespace Serenity.Services;

/// <summary>
/// Base class for behaviors implementing save and delete async behavior interfaces
/// </summary>
public abstract class BaseSaveDeleteBehaviorAsync : BaseSaveBehaviorAsync, IDeleteBehaviorAsync
{
    /// <inheritdoc/>
    public virtual Task OnPrepareQueryAsync(IDeleteRequestHandler handler, SqlQuery query,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnValidateRequestAsync(IDeleteRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnBeforeDeleteAsync(IDeleteRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnAfterDeleteAsync(IDeleteRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnAuditAsync(IDeleteRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public virtual Task OnReturnAsync(IDeleteRequestHandler handler,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
