namespace Serenity.Services;

/// <summary>
/// Base class for types implementing <see cref="IDeleteBehaviorAsync"/>
/// </summary>
public abstract class BaseDeleteBehaviorAsync : IDeleteBehaviorAsync
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
